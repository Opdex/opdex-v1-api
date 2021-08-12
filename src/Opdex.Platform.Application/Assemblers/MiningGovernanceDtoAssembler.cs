using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public class MiningGovernanceDtoAssembler : IModelAssembler<MiningGovernance, MiningGovernanceDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Token, TokenDto> _tokenAssembler;

        public MiningGovernanceDtoAssembler(IMediator mediator, IModelAssembler<Token, TokenDto> tokenAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _tokenAssembler = tokenAssembler ?? throw new ArgumentNullException(nameof(tokenAssembler));
        }

        public async Task<MiningGovernanceDto> Assemble(MiningGovernance governance)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(governance.TokenId));
            var tokenDto = await _tokenAssembler.Assemble(token);

            const uint miningPoolsPerYear = GovernanceConstants.MiningPoolsFundedPerYear;
            const uint maxNominations = GovernanceConstants.MaxNominations;

            var currentBlock = await _mediator.Send(new RetrieveLatestBlockQuery());
            var remainingNominationPeriodBlocks = governance.NominationPeriodEnd > currentBlock.Height
                ? governance.NominationPeriodEnd - currentBlock.Height
                : 0;

            return new MiningGovernanceDto
            {
                Address = governance.Address,
                MinedToken = tokenDto,
                PeriodEndBlock = governance.NominationPeriodEnd,
                PeriodRemainingBlocks = remainingNominationPeriodBlocks,
                PeriodBlockDuration = governance.MiningDuration,
                MiningPoolRewardPerPeriod = governance.MiningPoolReward,
                PeriodsUntilRewardReset = (miningPoolsPerYear - governance.MiningPoolsFunded) / maxNominations,
                TotalRewardsPerPeriod = (governance.MiningPoolReward.ToBigInteger() * maxNominations).ToString()
            };
        }
    }
}
