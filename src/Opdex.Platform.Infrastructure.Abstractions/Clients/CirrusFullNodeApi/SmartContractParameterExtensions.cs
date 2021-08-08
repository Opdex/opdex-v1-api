using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi
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
