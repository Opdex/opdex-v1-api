using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets
{
    public class PersistMarketCommand : IRequest<long>
    {
        public PersistMarketCommand(Market market)
        {
            Market = market ?? throw new ArgumentNullException(nameof(market));
        }
        
        public Market Market { get; }
    }
}