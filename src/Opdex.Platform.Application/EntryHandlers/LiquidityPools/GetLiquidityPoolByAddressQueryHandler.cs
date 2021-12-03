using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools;

public class GetLiquidityPoolByAddressQueryHandler: IRequestHandler<GetLiquidityPoolByAddressQuery, LiquidityPoolDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IModelAssembler<LiquidityPool, LiquidityPoolDto> _assembler;

    public GetLiquidityPoolByAddressQueryHandler(IMediator mediator, IMapper mapper, IModelAssembler<LiquidityPool, LiquidityPoolDto> assembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    }

    public async Task<LiquidityPoolDto> Handle(GetLiquidityPoolByAddressQuery request, CancellationToken cancellationToken)
    {
        var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Address), cancellationToken);

        var poolDto = await _assembler.Assemble(pool);

        return poolDto;
    }
}