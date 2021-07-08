using MediatR;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Governances
{
    public class MakeMiningGovernanceCommand : IRequest<long>
    {
        public MakeMiningGovernanceCommand(MiningGovernance miningGovernance)
        {
            MiningGovernance = miningGovernance ?? throw new ArgumentNullException(nameof(miningGovernance));
        }

        public MiningGovernance MiningGovernance { get; }
    }
}
