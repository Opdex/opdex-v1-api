using MediatR;
using Opdex.Platform.Domain.Models.Governances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Governances
{
    public class MakeMiningGovernanceCommand : IRequest<long>
    {
        public MakeMiningGovernanceCommand(MiningGovernance miningGovernance, ulong blockHeight, bool rewind = false)
        {
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
            BlockHeight = blockHeight;
            Rewind = rewind;
        }

        public MiningGovernance MiningGovernance { get; }
        public ulong BlockHeight { get; }
        public bool Rewind { get; }
    }
}
