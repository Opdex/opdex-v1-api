using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets;

public class MakeMarketSummaryCommand : IRequest<ulong>
{
    public MakeMarketSummaryCommand(MarketSummary summary)
    {
        Summary = summary ?? throw new ArgumentNullException(nameof(summary), "Market summary must be set.");
    }

    public MarketSummary Summary { get; }
}
