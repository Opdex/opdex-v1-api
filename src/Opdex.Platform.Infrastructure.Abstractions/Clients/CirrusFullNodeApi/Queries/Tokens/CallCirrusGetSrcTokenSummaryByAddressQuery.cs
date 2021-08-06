using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens
{
    public class CallCirrusGetSrcTokenSummaryByAddressQuery : IRequest<TokenContractSummary>
    {
        public CallCirrusGetSrcTokenSummaryByAddressQuery(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
        }

        public string Address { get; }
    }
}
