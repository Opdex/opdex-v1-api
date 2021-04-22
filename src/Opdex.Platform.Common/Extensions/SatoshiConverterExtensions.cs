using System;

namespace Opdex.Platform.Common.Extensions
{
    /// <summary>
    /// Extension helpers to convert ulong satoshi amounts to decimals and back
    /// </summary>
    public static class SatoshiConverterExtension
    {
        public static long DecimalsToSatoshis(this int decimals) => (int)Math.Pow(10, decimals);
        public static long DecimalsToSatoshis(this short decimals) => (int)Math.Pow(10, decimals);
        
        public static decimal ToDecimal(this ulong sats, int decimals)
        {
            return sats / (decimal)decimals.DecimalsToSatoshis();
        }
        
        public static ulong ToSatoshis(this decimal value, int decimals)
        {
            var sats = decimals.DecimalsToSatoshis();
            return (ulong)Math.Floor(value * sats);
        }
    }
}