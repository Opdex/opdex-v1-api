using System;

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
        /// Compares the value of two strings while ignoring casing.
        /// </summary>
        /// <param name="value">The string value to extend from</param>
        /// <param name="comparison">The string value to compare.</param>
        /// <returns>Boolean value as success result.</returns>
        public static bool EqualsIgnoreCase(this string value, string comparison)
        {
            return value.Equals(comparison, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
