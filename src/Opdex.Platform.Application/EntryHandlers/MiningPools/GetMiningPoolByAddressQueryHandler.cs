using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools;

public class GetMiningPoolByAddressQueryHandler : IRequestHandler<GetMiningPoolByAddressQuery, MiningPoolDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<MiningPool, MiningPoolDto> _miningPoolAssembler;

    public GetMiningPoolByAddressQueryHandler(IMediator mediator, IModelAssembler<MiningPool, MiningPoolDto> miningPoolAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _miningPoolAssembler = miningPoolAssembler ?? throw new ArgumentNullException(nameof(miningPoolAssembler));
    }

    public async Task<MiningPoolDto> Handle(GetMiningPoolByAddressQuery request, CancellationToken cancellationToken)
    {
        var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.MiningPool, findOrThrow: true), cancellationToken);
        var dto = await _miningPoolAssembler.Assemble(miningPool);
        return dto;
    }
}
