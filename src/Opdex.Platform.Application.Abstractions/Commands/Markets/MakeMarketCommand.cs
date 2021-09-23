using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Commands.Markets
{
    /// <summary>
    /// Create a make market command to upsert and persist a market instance.
    /// </summary>
    public class MakeMarketCommand : IRequest<long>
    {
        /// <summary>
        /// Constructor to create a make market command.
        /// </summary>
        /// <param name="market">The market domain model to upsert.</param>
        /// <param name="blockHeight">The block height ot refresh optional properties at.</param>
        /// <param name="refreshOwner">Flag to refresh the owner property of the market contract, default is false.</param>
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
