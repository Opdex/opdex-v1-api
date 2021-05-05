using System;
using System.Runtime.CompilerServices;

namespace Opdex.Platform.Common.Extensions
{
    /// <summary>
    /// Extension helpers to convert ulong satoshi amounts to decimals and back
    /// </summary>
    public static class SatoshiConverterExtension
    {
        public static long DecimalsToSatoshis(this int decimals) => (int)Math.Pow(10, decimals);
        public static ulong DecimalsToSatoshis(this short decimals) => (ulong)Math.Pow(10, decimals);
        
        public static decimal ToDecimal(this ulong sats, int decimals)
        {
            return sats / (decimal)decimals.DecimalsToSatoshis();
        }
        
        public static ulong ToSatoshis(this decimal value, int decimals)
        {
            var sats = decimals.DecimalsToSatoshis();
            return (ulong)Math.Floor(value * sats);
        }

        public static string ToSatoshis(this string value, int decimals)
        {
            var decimalIndex = value.IndexOf('.');
            
            var existingDecimalPlaces = value.Length - decimalIndex - 1;

            if (existingDecimalPlaces > decimals)
            {
                value = value.Substring(decimalIndex + decimals, value.Length - (existingDecimalPlaces - decimals));
            }
            
            if (value.Contains('.'))
            {
                value = value.Replace(".", "");
            }

            var remainder = decimals >= existingDecimalPlaces
                ? decimals - existingDecimalPlaces
                : 0;
            
            value = value.PadRight(value.Length + remainder, '0');

            return value.TrimStart('0').Length > 0 ? value.TrimStart('0') : "0";
        }
        
        public static string InsertDecimal(this string value, int decimals)
        {
            // +1 to have a leading 0
            var padded = value.PadLeft(decimals + 1, '0');
            return padded.Insert(padded.Length - decimals, ".");
        }

        public static string RemoveTrailingZeros(this string value)
        {
            return value.TrimEnd('0');
        }

        // Todo: Implement rounding
        public static string CutPrecisely(this string value, int precision)
        {
            var values = value.Split('.');
            var integer = values[0];
            var fraction = values[1];
            
            var sliced = fraction.Substring(0, precision);
            
            return $"{integer}.{sliced}";
        }
        
        public static decimal ToRoundedDecimal(this string value, int precision, int decimals)
        {
            if (!value.Contains('.'))
            {
                value = value.InsertDecimal(decimals);
            }

            var parsed = decimal.TryParse(value.CutPrecisely(precision), out var roundedDecimal);

            return !parsed ? 0m : roundedDecimal;
        }
    }
}