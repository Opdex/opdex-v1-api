using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsValid<T>(this T value) where T : Enum
        {
            return Enum.IsDefined(value.GetType(), value);
        }
    }
}
