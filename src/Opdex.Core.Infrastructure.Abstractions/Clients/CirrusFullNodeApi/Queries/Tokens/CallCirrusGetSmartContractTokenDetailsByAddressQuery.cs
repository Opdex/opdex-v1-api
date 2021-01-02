using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSmartContractTokenDetailsByAddressQuery : IRequest<TokenDto>
    {
        public CallCirrusGetSmartContractTokenDetailsByAddressQuery(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
        }
        
        public string Address { get; }
    }
}