
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.ViewModels
{
    public class CoordinateViewModel
    {
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }

        public CoordinateViewModel(Coordinate coordinate)
        {
            Longitude = coordinate.X;
            Latitude = coordinate.Y;
        }
    }
}