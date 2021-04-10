using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;

namespace Opdex.Platform.Application.Assemblers
{
    public class PoolDtoAssembler: IModelAssembler<Pool, PoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public PoolDtoAssembler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<PoolDto> Assemble(Pool pool)
        {
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(pool.TokenId));
            
            var poolDto = _mapper.Map<PoolDto>(pool);
            var tokenDto = _mapper.Map<TokenDto>(token);

            poolDto.Token = tokenDto;

            return poolDto;
        }
    }
}