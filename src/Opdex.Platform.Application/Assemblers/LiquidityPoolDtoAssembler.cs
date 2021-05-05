using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Assemblers
{
    public class LiquidityPoolDtoAssembler: IModelAssembler<LiquidityPool, LiquidityPoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public LiquidityPoolDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<LiquidityPoolDto> Assemble(LiquidityPool pool)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(pool.TokenId));
            
            var poolDto = _mapper.Map<LiquidityPoolDto>(pool);
            var tokenDto = _mapper.Map<TokenDto>(token);

            poolDto.Token = tokenDto;

            return poolDto;
        }
    }
}