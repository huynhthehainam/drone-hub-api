


using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MiSmart.DAL.DatabaseContexts;
using NetTopologySuite.Geometries;

namespace MiSmart.API.Helpers
{
    public static class DistanceHelper
    {
        public static double CalculateDistance(DatabaseContext databaseContext, Point point1, Point point2)
        {
            var firstLng = point1.X;
            var firstLat = point1.Y;
            var secondLng = point2.X;
            var secondLat = point2.Y;
            var distance = 0.0;
            using (var databaseCommand = databaseContext.Database.GetDbConnection().CreateCommand())
            {

                databaseCommand.CommandText = @$"select ST_Distance(st_transform( st_geomfromtext ('point({firstLng} {firstLat})', 4326), 3857 ),
st_transform(st_geomfromtext ('point({secondLng} {secondLat})',4326) , 3857)) * cosd({firstLat})
";
                databaseCommand.CommandType = CommandType.Text;
                databaseContext.Database.OpenConnection();
                using (var result = databaseCommand.ExecuteReader())
                {
                    while (result.Read())
                    {
                        var parsed = Double.TryParse(result[0].ToString(), out distance);
                        break;
                    }
                }
            }
            return distance;
        }
    }
}