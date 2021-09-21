using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    /// <summary>
    /// Create a new mining governance record when one does not already exist.
    /// </summary>
    public class CreateMiningGovernanceCommand : IRequest<long>
    {
        /// <summary>
        /// Constructor to create a mining governance command.
        /// </summary>
        /// <param name="governance">The mining governance contract address.</param>
        /// <param name="stakingTokenId">The staking or mined token in the governance.</param>
        /// <param name="blockHeight">The block height the governance was created at.</param>
        public CreateMiningGovernanceCommand(Address governance, long stakingTokenId, ulong blockHeight)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance), "Governance address must be provided.");
            }

            if (stakingTokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(stakingTokenId), "Staking token id must be greater than zero.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Governance = governance;
            BlockHeight = blockHeight;
            StakingTokenId = stakingTokenId;
        }

        public Address Governance { get; }
        public long StakingTokenId { get; }
        public ulong BlockHeight { get; }
    }
}
