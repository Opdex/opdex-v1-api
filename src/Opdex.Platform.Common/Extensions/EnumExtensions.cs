using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Validates that a given enum value against its type.
        /// </summary>
        /// <param name="value">The value of the enum.</param>
        /// <returns>Boolean as success</returns>
        public static bool IsValid<T>(this T value) where T : Enum
        {
            return Enum.IsDefined(value.GetType(), value);
        }
    }
}
