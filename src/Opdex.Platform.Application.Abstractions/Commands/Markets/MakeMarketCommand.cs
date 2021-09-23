using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    public class MakeMarketCommand : IRequest<long>
    {
        public MakeMarketCommand(Market market, ulong blockHeight, bool refreshOwner = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Market = market ?? throw new ArgumentNullException(nameof(market), "Market must be provided.");
            BlockHeight = blockHeight;
            RefreshOwner = refreshOwner && !market.IsStakingMarket; // Only non staking markets
        }

        public Market Market { get; }
        public ulong BlockHeight { get; }
        public bool RefreshOwner { get; }
        public bool Refresh => RefreshOwner;
    }
}
