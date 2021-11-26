using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations
{
    public class RetrieveCirrusMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceContractNominationSummary>>
    {
        public RetrieveCirrusMiningGovernanceNominationsQuery(Address address, ulong blockHeight)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }

            Address = address;
            BlockHeight = blockHeight;
        }

        public Address Address { get; }
        public ulong BlockHeight { get; }
    }
}
