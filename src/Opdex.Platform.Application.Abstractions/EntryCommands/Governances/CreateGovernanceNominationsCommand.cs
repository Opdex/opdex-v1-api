using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    public class CreateGovernanceNominationsCommand : IRequest<bool>
    {
        public CreateGovernanceNominationsCommand(Address governance, ulong blockHeight)
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
        }

        public Address Governance { get; }
        public ulong BlockHeight { get; }
    }
}
