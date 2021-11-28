using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances
{
    public class CreateMiningGovernanceNominationsCommand : IRequest<bool>
    {
        public CreateMiningGovernanceNominationsCommand(Address miningGovernance, ulong blockHeight)
        {
            if (miningGovernance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningGovernance), "Mining governance address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MiningGovernance = miningGovernance;
            BlockHeight = blockHeight;
        }

        public Address MiningGovernance { get; }
        public ulong BlockHeight { get; }
    }
}
