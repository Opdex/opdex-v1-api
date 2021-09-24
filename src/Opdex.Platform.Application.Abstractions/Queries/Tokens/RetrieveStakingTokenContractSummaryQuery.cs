using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveStakingTokenContractSummaryQuery : IRequest<StakingTokenContractSummary>
    {
        public RetrieveStakingTokenContractSummaryQuery(Address token, ulong blockHeight, bool includeGenesis = false,
                                                        bool includePeriodIndex = false, bool includeVault = false,
                                                        bool includeMiningGovernance = false, bool includePeriodDuration = false)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Token = token;
            BlockHeight = blockHeight;
            IncludeGenesis = includeGenesis;
            IncludePeriodIndex = includePeriodIndex;
            IncludeVault = includeVault;
            IncludeMiningGovernance = includeMiningGovernance;
            IncludePeriodDuration = includePeriodDuration;
        }

        public Address Token { get; }
        public ulong BlockHeight { get; }
        public bool IncludeGenesis { get; }
        public bool IncludePeriodIndex { get; }
        public bool IncludeVault { get; }
        public bool IncludeMiningGovernance { get; }
        public bool IncludePeriodDuration { get; }
    }
}
