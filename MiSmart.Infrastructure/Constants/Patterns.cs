using System;

namespace MiSmart.Infrastructure.Constants
{
    public class Patterns
    {
        public const String OffsetPattern = @"^(?<sign>\+|\-)(?<hour>(((0)([0-9]))|((1)([0-2]))))(?<min>([0-5])([0-9]))$";
        public const String HexColorPattern = "^#(?:[0-9a-fA-F]{3}){1,2}$";
        public const String AirportNamePattern = "^([A-Z]{3})$";
    }
}