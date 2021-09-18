using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    public class CreateMiningGovernanceCommand : IRequest<long>
    {
        public CreateMiningGovernanceCommand(Address governance, ulong blockHeight, bool isUpdate)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance), "Governance address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Governance = governance;
            BlockHeight = blockHeight;
            IsUpdate = isUpdate;
        }

        public Address Governance { get; }
        public ulong BlockHeight { get; }
        public bool IsUpdate { get; }
    }
}
