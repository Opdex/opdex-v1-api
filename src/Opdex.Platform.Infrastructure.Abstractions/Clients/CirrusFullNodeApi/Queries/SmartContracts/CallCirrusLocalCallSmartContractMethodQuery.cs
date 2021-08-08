using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodQuery : IRequest<LocalCallResponseDto>
    {
        public CallCirrusLocalCallSmartContractMethodQuery(string address, string method, string[] parameters = null)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (!method.HasValue())
            {
                throw new ArgumentNullException(nameof(method));
            }

            Address = address;
            Method = method;
            Parameters = parameters ?? new string[0];
        }
        
        public string Address { get; }
        public string Method { get; }
        public string[] Parameters { get; }
    }
}