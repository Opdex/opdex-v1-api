using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets
{
    public class PersistMarketRouterCommand : IRequest<bool>
    {
        public PersistMarketRouterCommand(MarketRouter router)
        {
            Router = router ?? throw new ArgumentNullException(nameof(router));
        }
        
        public MarketRouter Router { get; }
    }
}