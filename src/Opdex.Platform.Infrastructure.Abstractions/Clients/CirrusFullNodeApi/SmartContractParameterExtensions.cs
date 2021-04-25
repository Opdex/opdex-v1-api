using System;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi
{
    public static class SmartContractParameterExtensions
    {
        public static string ToSmartContractParameter(this object parameter, SmartContractParameterType type)
        {
            if (type == SmartContractParameterType.Unknown)
            {
                throw new ArgumentException(nameof(type));
            }

            return $"{(uint)type}#{parameter}";
        }
    }
}