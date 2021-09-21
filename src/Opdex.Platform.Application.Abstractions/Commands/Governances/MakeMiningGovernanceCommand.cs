using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Governances
{
    /// <summary>
    /// Make a mining governance by persisting as is or optionally refresh select properties.
    /// </summary>
    public class MakeMiningGovernanceCommand : IRequest<long>
    {
        /// <summary>
        /// Constructor to create the make mining governance command.
        /// </summary>
        /// <param name="miningGovernance">The mining governance domain model to persist and/or update.</param>
        /// <param name="blockHeight">The block height to refresh properties at.</param>
        /// <param name="refreshNominationPeriodEnd">Flag to refresh the nomination period end property using the contract's state, default is false.</param>
        /// <param name="refreshMiningPoolsFunded">Flag to refresh the mining pools funded property using the contract's state, default is false.</param>
        /// <param name="refreshMiningPoolReward">Flag to refresh the mining pool reward property using the contract's state, default is false.</param>
        /// <param name="refreshMiningDuration">Flag to refresh the mining duration property using the contract's state, default is false.</param>
        /// <param name="refreshMinedToken">Flag to refresh the mined property token using the contract's state, default is false.</param>
        public MakeMiningGovernanceCommand(MiningGovernance miningGovernance, ulong blockHeight, bool refreshNominationPeriodEnd = false,
                                           bool refreshMiningPoolsFunded = false, bool refreshMiningPoolReward = false,
                                           bool refreshMiningDuration = false, bool refreshMinedToken = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance), "Mining governance must be provided.");
            BlockHeight = blockHeight;
            RefreshNominationPeriodEnd = refreshNominationPeriodEnd;
            RefreshMiningPoolsFunded = refreshMiningPoolsFunded;
            RefreshMiningPoolReward = refreshMiningPoolReward;
            RefreshMiningDuration = refreshMiningDuration;
            RefreshMinedToken = refreshMinedToken;
        }

        public MiningGovernance MiningGovernance { get; }
        public ulong BlockHeight { get; }
        public bool RefreshNominationPeriodEnd { get; }
        public bool RefreshMiningPoolsFunded { get; }
        public bool RefreshMiningPoolReward { get; }
        public bool RefreshMiningDuration { get; }
        public bool RefreshMinedToken { get; }
        public bool Refresh => RefreshNominationPeriodEnd || RefreshMiningPoolsFunded ||
                               RefreshMiningPoolReward || RefreshMiningDuration || RefreshMinedToken;

    }
}
