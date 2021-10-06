using MediatR;
using Opdex.Platform.Domain.Models.MarketTokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MarketTokens
{
    public class PersistMarketTokenCommand : IRequest<bool>
    {
        public PersistMarketTokenCommand(MarketToken marketToken)
        {
            MarketToken = marketToken ?? throw new ArgumentNullException(nameof(marketToken), "Market token must be provided.");
        }

        public MarketToken MarketToken { get; }
    }
}
