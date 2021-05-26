using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
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