using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryNominationsQuery : IRequest<IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        public CallCirrusGetMiningGovernanceSummaryNominationsQuery(Address address, ulong blockHeight)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
            BlockHeight = blockHeight;
        }

        public Address Address { get; }
        public ulong BlockHeight { get; }
    }
}
