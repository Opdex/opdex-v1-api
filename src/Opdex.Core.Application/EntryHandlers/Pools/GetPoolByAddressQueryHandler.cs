using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.EntryQueries.Pools;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Pools;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application.EntryHandlers.Pools
{
    public class GetPoolByAddressQueryHandler: IRequestHandler<GetPoolByAddressQuery, PoolDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<Pool, PoolDto> _assembler;
        
        public GetPoolByAddressQueryHandler(IMediator mediator, IMapper mapper, IModelAssembler<Pool, PoolDto> assembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }
        
        public async Task<PoolDto> Handle(GetPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrievePoolByAddressQuery(request.Address), cancellationToken);

            var poolDto = await _assembler.Assemble(pool);

            return poolDto;
        }
    }
}