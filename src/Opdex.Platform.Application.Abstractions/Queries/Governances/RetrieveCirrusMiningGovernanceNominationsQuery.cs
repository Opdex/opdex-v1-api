using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Governances
{
    public class RetrieveCirrusMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        public RetrieveCirrusMiningGovernanceNominationsQuery(string address)
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
