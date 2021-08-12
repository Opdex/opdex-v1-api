using Opdex.Platform.Common.Enums;
using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class SmartContractParameterExtensions
    {
        public static string ToSmartContractParameter(this object parameter, SmartContractParameterType type)
        {
            if (!type.IsValid() || type == SmartContractParameterType.Unknown)
            {
                throw new ArgumentOutOfRangeException(nameof(type), "Parameter type must be set.");
            }

            return $"{(uint)type}#{parameter}";
        }
    }
}
