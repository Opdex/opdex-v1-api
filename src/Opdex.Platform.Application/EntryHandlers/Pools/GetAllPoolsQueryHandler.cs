using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Pools
{
    public class GetAllPoolsQueryHandler : IRequestHandler<GetAllPoolsQuery, IEnumerable<LiquidityPoolDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;
        
        public GetAllPoolsQueryHandler(IMediator mediator, IMapper mapper, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public async Task<IEnumerable<LiquidityPoolDto>> Handle(GetAllPoolsQuery request, CancellationToken cancellationToken)
        {
            var query = new RetrieveAllPoolsQuery();
            
            var pools = await _mediator.Send(query, cancellationToken);

            var poolDtos = new List<LiquidityPoolDto>();

            foreach (var pool in pools)
            {
                var poolDto = await _assembler.Assemble(pool);
                poolDtos.Add(poolDto);
            }

            return poolDtos;
        }
    }
}