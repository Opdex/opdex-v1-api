using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    public class MakeMarketRouterCommand : IRequest<bool>
    {
        public MakeMarketRouterCommand(MarketRouter router)
        {
            Router = router ?? throw new ArgumentNullException(nameof(router));
        }
        
        public MarketRouter Router { get; }
    }
}