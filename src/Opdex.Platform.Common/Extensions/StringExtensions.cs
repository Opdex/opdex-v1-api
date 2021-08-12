using Opdex.Platform.Common.Constants;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Opdex.Platform.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Validates that a string has a value.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>Boolean value of validity.</returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validates that a string is a properly formatted decimal number.
        /// </summary>
        /// <param name="value">The value of the string to check.</param>
        /// <returns>Boolean value of validity.</returns>
        public static bool IsValidDecimalNumber(this string value)
        {
            if (!value.HasValue()) return false;

            var count = value.Count(x => x == '.');

            if (count > 1) return false;

            if (count == 0 && !value.Equals("0")) return false;

            return value.Replace(".", "").IsNumeric();
        }

        /// <summary>
        /// Validates that a string contains numeric digits only.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>Boolean value of validity.</returns>
        public static bool IsNumeric(this string value)
        {
            return value.HasValue() && Regex.IsMatch(value, @"^\d+$");
        }

        /// <summary>
        /// Compares the value of two strings while ignoring casing.
        /// </summary>
        /// <param name="value">The string value to extend from</param>
        /// <param name="comparison">The string value to compare.</param>
        /// <returns>Boolean value as success result.</returns>
        public static bool EqualsIgnoreCase(this string value, string comparison)
        {
            return value.Equals(comparison, StringComparison.InvariantCultureIgnoreCase);
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

        /// <summary>
        /// Returns the total fiat amount of a number of tokens with a known price per token.
        /// </summary>
        /// <param name="tokenAmount">The total number of tokens.</param>
        /// <param name="fiatPerToken">The fiat price per full token.</param>
        /// <param name="tokenSats">The number of satoshis per token.</param>
        /// <returns>The fiat amount as a decimal of total amount of tokens provided.</returns>
        public static decimal TotalFiat(this ulong tokenAmount, decimal fiatPerToken, ulong tokenSats)
        {
            return tokenAmount.ToString().TotalFiat(fiatPerToken, tokenSats);
        }

        /// <summary>
        /// Returns the total fiat amount of a number of tokens with a known price per token.
        /// </summary>
        /// <param name="tokenAmount">The total number of tokens.</param>
        /// <param name="fiatPerToken">The fiat price per full token.</param>
        /// <param name="tokenSats">The number of satoshis per token.</param>
        /// <returns>The fiat amount as a decimal of total amount of tokens provided.</returns>
        public static decimal TotalFiat(this string tokenAmount, decimal fiatPerToken, ulong tokenSats)
        {
            if (!tokenAmount.HasValue() || tokenAmount.Equals("0"))
            {
                return 0.00m;
            }

            if (!tokenAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(tokenAmount), "Must be a valid numeric number.");
            }

            const ulong fiatOffset = FiatConstants.Usd.Offset;

            // Offset the fiat decimal
            var fiatPerWithOffsetBigInt = Math.Floor(fiatPerToken * fiatOffset).ToString().ToBigInteger();

            // Attempt to parse token * fiatPer / sats as the fiat price including the fiat offset
            if (!decimal.TryParse((tokenAmount.ToBigInteger() * fiatPerWithOffsetBigInt / tokenSats).ToString(), out var fiatWithOffsetDecimal))
            {
                throw new Exception("Unable to parse fiat decimal including offset.");
            }

            // Remove the fiatOffset and round back to a decimal
            return Math.Round(fiatWithOffsetDecimal / fiatOffset, FiatConstants.Usd.Decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns the fiat amount per token.
        /// </summary>
        /// <param name="tokenAmount">The total number or supply of tokens.</param>
        /// <param name="fiatAmount">The total fiat amount of all tokens combined.</param>
        /// <param name="tokenSats">The amount of satoshis in the token.</param>
        /// <returns>Decimal representing fiat value per token.</returns>
        public static decimal FiatPerToken(this string tokenAmount, decimal fiatAmount, ulong tokenSats)
        {
            if (!tokenAmount.HasValue() || tokenAmount.Equals("0"))
            {
                return 0.00m;
            }

            if (!tokenAmount.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(tokenAmount), "Must be a valid numeric number.");
            }

            const ulong fiatOffset = FiatConstants.Usd.Offset;

            // Offset the fiat decimal
            var fiatWithOffsetBigInt = Math.Floor(fiatAmount * fiatOffset).ToString().ToBigInteger();

            // Attempt to parse fiatWithOffset * tokenSats / tokenAmount
            if (!decimal.TryParse((fiatWithOffsetBigInt * tokenSats / (tokenAmount.ToBigInteger())).ToString(), out var fiatWithOffsetDecimal))
            {
                throw new Exception("Unable to parse fiat decimal including offset.");
            }

            // Remove the fiat offset and round back to decimal
            return Math.Round(fiatWithOffsetDecimal / fiatOffset, FiatConstants.Usd.Decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Get the percentage change difference of two string values as satoshis.
        /// </summary>
        /// <param name="current">The current amount.</param>
        /// <param name="previous">The previous amount.</param>
        /// <param name="tokenSats">The amount of sats per token.</param>
        /// <returns>Decimal of percent change</returns>
        public static decimal PercentChange(this string current, string previous, ulong tokenSats)
        {
            if (!previous.HasValue() || previous.Equals("0"))
            {
                return 0m;
            }

            if (!previous.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(previous), "Invalid previous amount.");
            }

            if (!current.HasValue() || !current.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(current), "Invalid current amount.");
            }

            // Convert to Big Integer
            var currentBigInt = current.ToBigInteger();
            var previousBigInt = previous.ToBigInteger();

            // Apply offset of tokenSats
            var currentOffset = currentBigInt * tokenSats;
            var previousOffset = previousBigInt * tokenSats;

            // Find the percentage change with offset
            var change = (currentOffset - previousOffset) / previousBigInt * 100;

            // Try Parse
            if (!decimal.TryParse(change.ToString(), out var changeDecimalWithOffset))
            {
                throw new Exception("Unable to parse change decimal including offset.");
            }

            // Return percentage change without offset, rounded 2 places
            return Math.Round(changeDecimalWithOffset / tokenSats, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Get the percentage change difference of two decimal values.
        /// </summary>
        /// <param name="current">The current amount.</param>
        /// <param name="previous">The previous amount to compare to.</param>
        /// <returns>Percentage changed as decimal</returns>
        public static decimal PercentChange(this decimal current, decimal previous)
        {
            if (previous <= 0)
            {
                return 0.00m;
            }

            var usdDailyChange = (current - previous) / previous * 100;

            return Math.Round(usdDailyChange, 2, MidpointRounding.AwayFromZero);
        }
    }
}
