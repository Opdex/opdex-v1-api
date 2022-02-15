using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class MiningGovernanceDtoAssembler : IModelAssembler<MiningGovernance, MiningGovernanceDto>
{
    private readonly IMediator _mediator;

    public MiningGovernanceDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<MiningGovernanceDto> Assemble(MiningGovernance miningGovernance)
    {
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(miningGovernance.TokenId));

        const uint miningPoolsPerYear = MiningGovernanceConstants.MiningPoolsFundedPerYear;
        const uint maxNominations = MiningGovernanceConstants.MaxNominations;

        var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
        var remainingNominationPeriodBlocks = miningGovernance.NominationPeriodEnd > currentBlock.Height
            ? miningGovernance.NominationPeriodEnd - currentBlock.Height
            : 0;

        var totalRewardsPerPeriod = miningGovernance.MiningPoolReward * maxNominations;

        return new MiningGovernanceDto
        {
            Address = miningGovernance.Address,
            MinedToken = token.Address,
            PeriodEndBlock = miningGovernance.NominationPeriodEnd,
            PeriodRemainingBlocks = remainingNominationPeriodBlocks,
            PeriodBlockDuration = miningGovernance.MiningDuration,
            MiningPoolRewardPerPeriod = miningGovernance.MiningPoolReward.ToDecimal(token.Decimals),
            PeriodsUntilRewardReset = (miningPoolsPerYear - miningGovernance.MiningPoolsFunded) / maxNominations,
            TotalRewardsPerPeriod = totalRewardsPerPeriod.ToDecimal(token.Decimals),
            CreatedBlock = miningGovernance.CreatedBlock,
            ModifiedBlock = miningGovernance.ModifiedBlock
        };
    }
}
