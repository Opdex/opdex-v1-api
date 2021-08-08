using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryNominationsQuery : IRequest<IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        public CallCirrusGetMiningGovernanceSummaryNominationsQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public string Address { get; }
    }
}
