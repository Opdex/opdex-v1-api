using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernance
{
    public class RetrieveCirrusMiningGovernanceNominationsQuery : IRequest<IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        public RetrieveCirrusMiningGovernanceNominationsQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
        }
        
        public string Address { get; }
    }
}