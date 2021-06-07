using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    public class MakeMarketCommand : IRequest<long>
    {
        public MakeMarketCommand(Market market)
        {
            Market = market ?? throw new ArgumentNullException(nameof(market));
        }
        
        public Market Market { get; }
    }
}