
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MIConvexHull;
using Microsoft.Extensions.DependencyInjection;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.Infrastructure.ScheduledTasks;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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
                    var flightStats = databaseContext.FlightStats.Where(ww => !ww.IsBoundaryArchived && ww.SprayedIndexes != null && ww.SprayedIndexes.Count > 2).OrderByDescending(ww => ww.FlightTime).Take(10).ToList();
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    foreach (var flightStat in flightStats)
                    {
                        var sprayedIndexes = flightStat.SprayedIndexes;
                        sprayedIndexes.Sort();
                        var minIndex = sprayedIndexes.FirstOrDefault();
                        var maxIndex = sprayedIndexes.LastOrDefault();
                        List<Coordinate> coordinates = new List<Coordinate>();
                        for (var i = minIndex; i <= maxIndex; i += 1)
                        {
                            coordinates.Add(flightStat.FlywayPoints.Coordinates[i]);
                        }
                        var vertices = new List<MyVertex>();
                        foreach (var coordinate in coordinates)
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