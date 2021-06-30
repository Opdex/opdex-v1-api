using System;

namespace Opdex.Platform.Common.Extensions
{
    /// <summary>
    /// Extension helpers to convert ulong satoshi amounts to decimals and back
    /// </summary>
    public static class SatoshiConverterExtension
    {
        // public static ulong DecimalsToSatoshis(this int decimals) => (int)Math.Pow(10, decimals);
        public static ulong DecimalsToSatoshis(this int decimals) => (ulong)Math.Pow(10, decimals);

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

        // Todo: Implement rounding
        public static string CutPrecisely(this string value, int precision)
        {
            var values = value.Split('.');
            var integer = values[0];
            var fraction = values[1];

            var sliced = fraction.Substring(0, precision);

            return $"{integer}.{sliced}";
        }

        // Todo: Doesn't actually round
        public static decimal ToRoundedDecimal(this string value, int precision, int decimals)
        {
            if (!value.Contains('.'))
            {
                value = value.InsertDecimal(decimals);
            }

            var parsed = decimal.TryParse(value.CutPrecisely(precision), out var roundedDecimal);

            return !parsed ? 0m : roundedDecimal;
        }

        public static string Token0PerToken1(this ulong token0, string token1, ulong token1Sats)
        {
            return token0.ToString().Token0PerToken1(token1, token1Sats);
        }

        public static string Token0PerToken1(this string token0, ulong token1, ulong token1Sats)
        {
            return token0.Token0PerToken1(token1.ToString(), token1Sats);
        }

        public static string Token0PerToken1(this string token0, string token1, ulong token1Sats)
        {
            if (!token0.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(token0), $"{nameof(token0)} must be a numeric value.");
            }

            if (!token1.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(token1), $"{nameof(token1)} must be a numeric value.");
            }

            return token1.Equals("0")
                ? "0"
                : (token0.ToBigInteger() * token1Sats / token1.ToBigInteger()).ToString();
        }
    }
}
