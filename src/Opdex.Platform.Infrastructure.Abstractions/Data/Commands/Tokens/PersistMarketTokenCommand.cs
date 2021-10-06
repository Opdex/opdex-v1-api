using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens
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
