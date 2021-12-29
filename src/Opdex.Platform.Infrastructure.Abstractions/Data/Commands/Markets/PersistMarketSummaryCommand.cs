using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;

public class PersistMarketSummaryCommand : IRequest<ulong>
{
    public PersistMarketSummaryCommand(MarketSummary summary)
    {
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));
    }

    public MarketSummary Summary { get; }
}
