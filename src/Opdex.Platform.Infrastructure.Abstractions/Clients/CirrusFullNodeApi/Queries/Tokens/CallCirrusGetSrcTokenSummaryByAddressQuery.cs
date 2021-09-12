using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenSummaryByAddressQuery : IRequest<TokenContractSummary>
    {
        public CallCirrusGetSrcTokenSummaryByAddressQuery(Address address)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address));
        }

        public Address Address { get; }
    }
}
