

using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace MiSmart.DAL.Extensions
{
    public static class GeometryExtensions
    {
        private static readonly CoordinateSystemServices _coordinateSystemServices
            = new CoordinateSystemServices(
                new Dictionary<Int32, String>
                {
                    [4326] = GeographicCoordinateSystem.WGS84.WKT,
                    [3857] = "PROJCS[\"WGS 84 / World Mercator\",GEOGCS[\"WGS 84 sphere\",DATUM[\"WGS_1984 sphere\",SPHEROID[\"WGS 84 sphere\",6378137,0.0]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"Easting\",EAST],AXIS[\"Northing\",NORTH]]",
                    [2855] =
                @"
                    PROJCS[""NAD83(HARN) / Washington North"",
                        GEOGCS[""NAD83(HARN)"",
                            DATUM[""NAD83_High_Accuracy_Regional_Network"",
                                SPHEROID[""GRS 1980"",6378137,298.257222101,
                                    AUTHORITY[""EPSG"",""7019""]],
                                AUTHORITY[""EPSG"",""6152""]],
                            PRIMEM[""Greenwich"",0,
                                AUTHORITY[""EPSG"",""8901""]],
                            UNIT[""degree"",0.01745329251994328,
                                AUTHORITY[""EPSG"",""9122""]],
                            AUTHORITY[""EPSG"",""4152""]],
                        PROJECTION[""Lambert_Conformal_Conic_2SP""],
                        PARAMETER[""standard_parallel_1"",48.73333333333333],
                        PARAMETER[""standard_parallel_2"",47.5],
                        PARAMETER[""latitude_of_origin"",47],
                        PARAMETER[""central_meridian"",-120.8333333333333],
                        PARAMETER[""false_easting"",500000],
                        PARAMETER[""false_northing"",0],
                        UNIT[""metre"",1,
                            AUTHORITY[""EPSG"",""9001""]],
                        AUTHORITY[""EPSG"",""2855""]]
                "
                });

        public static Geometry ProjectTo(this Geometry geometry, Int32 srid)
        {
            CoordinateSystemFactory c = new CoordinateSystemFactory();
            var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

            var result = geometry.Copy();
            result.Apply(new MathTransformFilter(transformation.MathTransform));

            return result;
        }

        private class MathTransformFilter : ICoordinateSequenceFilter
        {
            private readonly MathTransform _transform;

            public MathTransformFilter(MathTransform transform)
                => _transform = transform;

            public Boolean Done => false;
            public Boolean GeometryChanged => true;

            public void Filter(CoordinateSequence seq, Int32 i)
            {
                var x = seq.GetX(i);
                var y = seq.GetY(i);
                var z = seq.GetZ(i);
                _transform.Transform(ref x, ref y, ref z);
                seq.SetX(i, x);
                seq.SetY(i, y);
                seq.SetZ(i, z);
            }
        }
    }
}