using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class MiningGovernanceDtoAssembler : IModelAssembler<MiningGovernance, MiningGovernanceDto>
    {
        private readonly IMediator _mediator;

        public MiningGovernanceDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<MiningGovernanceDto> Assemble(MiningGovernance governance)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(governance.TokenId));

            const uint miningPoolsPerYear = GovernanceConstants.MiningPoolsFundedPerYear;
            const uint maxNominations = GovernanceConstants.MaxNominations;

            var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
            var remainingNominationPeriodBlocks = governance.NominationPeriodEnd > currentBlock.Height
                ? governance.NominationPeriodEnd - currentBlock.Height
                : 0;

            var totalRewardsPerPeriod = governance.MiningPoolReward.ToBigInteger() * maxNominations;

            return new MiningGovernanceDto
            {
                Address = governance.Address,
                MinedToken = token.Address,
                PeriodEndBlock = governance.NominationPeriodEnd,
                PeriodRemainingBlocks = remainingNominationPeriodBlocks,
                PeriodBlockDuration = governance.MiningDuration,
                MiningPoolRewardPerPeriod = governance.MiningPoolReward.InsertDecimal(token.Decimals),
                PeriodsUntilRewardReset = (miningPoolsPerYear - governance.MiningPoolsFunded) / maxNominations,
                TotalRewardsPerPeriod = totalRewardsPerPeriod.ToString().InsertDecimal(token.Decimals)
            };
        }
    }
}
