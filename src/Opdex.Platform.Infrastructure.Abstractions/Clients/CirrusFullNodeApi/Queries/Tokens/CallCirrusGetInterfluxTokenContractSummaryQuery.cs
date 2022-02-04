using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

public class CallCirrusGetInterfluxTokenContractSummaryQuery : IRequest<InterfluxTokenContractSummary>
{
    public CallCirrusGetInterfluxTokenContractSummaryQuery(Address token, ulong blockHeight)
    {
        if (token == Address.Empty) throw new ArgumentNullException(nameof(token), "Token address must be provided.");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        Token = token;
        BlockHeight = blockHeight;
    }
    public Address Token { get; }
    public ulong BlockHeight { get; }
}
