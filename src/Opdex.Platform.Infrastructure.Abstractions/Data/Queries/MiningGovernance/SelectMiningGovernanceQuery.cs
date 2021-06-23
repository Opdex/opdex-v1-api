using System;
using MediatR;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance
{
    public class SelectMiningGovernanceQuery : FindQuery<Domain.Models.MiningGovernance>
    {
        public SelectMiningGovernanceQuery(bool findOrThrow = true) : base(findOrThrow)
        {
        }
    }
}