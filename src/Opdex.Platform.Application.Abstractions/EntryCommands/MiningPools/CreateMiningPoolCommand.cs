using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    /// <summary>
    /// Create a mining pool command to persist a new mining pool tied to an associated staking liquidity pool.
    /// </summary>
    public class CreateMiningPoolCommand : IRequest<ulong>
    {
        /// <summary>
        /// Constructor to create a create mining pool command.
        /// </summary>
        /// <param name="liquidityPoolAddress">The address of the mining pool's associated liquidity pool contract.</param>
        /// <param name="liquidityPoolId">The internal liquidity pool Id.</param>
        /// <param name="blockHeight">The block height the mining pool was created at.</param>
        public CreateMiningPoolCommand(Address liquidityPoolAddress, ulong liquidityPoolId, ulong blockHeight)
        {
            if (liquidityPoolAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPoolAddress), "Liquidity pool address must be provided.");
            }

            if (liquidityPoolId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than zero.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            LiquidityPoolAddress = liquidityPoolAddress;
            LiquidityPoolId = liquidityPoolId;
            BlockHeight = blockHeight;
        }

        public Address LiquidityPoolAddress { get; }
        public ulong LiquidityPoolId { get; }
        public ulong BlockHeight { get; }
    }
}
