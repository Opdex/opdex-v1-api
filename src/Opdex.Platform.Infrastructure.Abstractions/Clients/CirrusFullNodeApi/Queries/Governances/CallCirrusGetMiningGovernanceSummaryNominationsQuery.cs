using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryNominationsQuery : IRequest<IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        public CallCirrusGetMiningGovernanceSummaryNominationsQuery(string address, ulong blockHeight)
        {
            if (!address.HasValue())
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

        public string Address { get; }
        public ulong BlockHeight { get; }
    }
}
