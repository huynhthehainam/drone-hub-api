
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.ScheduledTasks;
using MIConvexHull;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite;

namespace MiSmart.API.ScheduledTasks
{
    public class MyVertex : IVertex2D
    {
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }

        public double X => Longitude;

        public double Y => Latitude;


    }

    public class UpdatingFlightStatBoundary : CronJobService
    {
        private IServiceProvider serviceProvider;
        public UpdatingFlightStatBoundary(IScheduleConfig<UpdatingFlightStatBoundary> options, IServiceProvider serviceProvider) : base(options)
        {
            this.serviceProvider = serviceProvider;
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope())
            {

                using (DatabaseContext databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var flightStats = databaseContext.FlightStats.Where(ww => !ww.IsBoundaryArchived).OrderByDescending(ww => ww.FlightTime).Take(10).ToList();
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    foreach (var flightStat in flightStats)
                    {
                        var vertices = new List<MyVertex>();
                        foreach (var coordinate in flightStat.FlywayPoints.Coordinates)
                        {

                            vertices.Add(new MyVertex { Longitude = coordinate.X, Latitude = coordinate.Y });
                        }
                        var convexHullResult = ConvexHull.Create2D(vertices);
                        if (convexHullResult != null)
                        {
                            var convexHullPoints = convexHullResult.Result;
                            if (convexHullPoints != null)
                            {
                                List<Coordinate> coords = new List<Coordinate>();
                                foreach (var point in convexHullPoints)
                                {
                                    coords.Add(new Coordinate(point.Longitude, point.Latitude));
                                }
                                if (coords.Count > 2)
                                {
                                    coords.Add(new Coordinate(coords[0].X, coords[0].Y));
                                    var boundary = geometryFactory.CreatePolygon(coords.ToArray()); ;
                                    flightStat.Boundary = boundary;

                                }
                            }
                        }
                        flightStat.IsBoundaryArchived = true;
                        databaseContext.Update(flightStat);
                        databaseContext.SaveChanges();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}