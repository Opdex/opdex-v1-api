using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models;
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

            return new MiningGovernanceDto
            {
                Address = governance.Address,
                MinedToken = tokenDto,
                NominationPeriodEnd = governance.NominationPeriodEnd,
                MiningDuration = governance.MiningDuration,
                MiningPoolsFunded = governance.MiningPoolsFunded,
                MiningPoolReward = governance.MiningPoolReward
            };
        }
    }
}
