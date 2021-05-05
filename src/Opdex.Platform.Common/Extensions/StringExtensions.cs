using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string ToleranceAsSatoshis(this string value, decimal tolerance)
        {
            tolerance = Math.Round(tolerance, 4);
            
            if (tolerance > .9999m || tolerance < .0001m)
            {
                throw new Exception("Invalid tolerance, .0001 - .9999 supported");
            }

            if (!value.HasValue() || value.Contains('.'))
            {
                throw new Exception("Invalid value, must be a value in satoshis.");
            }
            
            const int offset = 10_000;
            var offsetPercentage = value.ToBigInteger() / offset;
            var toleranceOffset = (ulong)(Math.Round(1 - tolerance, 4) * offset);
            
            return (offsetPercentage * toleranceOffset).ToString();
        }
    }
}