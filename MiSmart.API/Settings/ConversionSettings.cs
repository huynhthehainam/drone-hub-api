
using MiSmart.DAL.Models;
using System;
using System.Collections.Generic;

namespace MiSmart.API.Settings
{

    public class AreaUnitConversion
    {
        public AreaUnit From { get; set; }
        public AreaUnit To { get; set; }
        public Double Value { get; set; }
    }
    public class LengthUnitConversion
    {
        public LengthUnit From { get; set; }
        public LengthUnit To { get; set; }
        public Double Value { get; set; }
    }
    public class ConversionSettings
    {
        public List<AreaUnitConversion> AreaConversions { get; set; } = new List<AreaUnitConversion>();
        public List<LengthUnitConversion> LengthConversions { get; set; } = new List<LengthUnitConversion>();
    }
}