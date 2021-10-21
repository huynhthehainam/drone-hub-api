
using MiSmart.DAL.Models;
using System;
using System.Collections.Generic;

namespace MiSmart.API.Settings
{

    public class UnitConversion
    {
        public AreaUnit From { get; set; }
        public AreaUnit To { get; set; }
        public Double Value { get; set; }
    }
    public class ConversionSettings
    {
        public List<UnitConversion> AreaConversions { get; set; } = new List<UnitConversion>();
        public List<UnitConversion> LengthConversions { get; set; } = new List<UnitConversion>();
    }
}