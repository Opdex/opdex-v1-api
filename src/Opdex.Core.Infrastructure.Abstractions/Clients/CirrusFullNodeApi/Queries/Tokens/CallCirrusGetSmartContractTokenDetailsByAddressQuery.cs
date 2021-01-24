using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSmartContractTokenDetailsByAddressQuery : IRequest<Token>
    {
        public CallCirrusGetSmartContractTokenDetailsByAddressQuery(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
        }
        
        public string Address { get; }
    }
}