using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Application.Abstractions.Models.Admins;
using Opdex.Platform.Application.Abstractions.Queries.Admins;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Admins;

public class GetAdminByAddressQueryHandler : IRequestHandler<GetAdminByAddressQuery, AdminDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public GetAdminByAddressQueryHandler(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AdminDto> Handle(GetAdminByAddressQuery request, CancellationToken cancellationToken)
    {
        var admin = await _mediator.Send(new RetrieveAdminByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);

        return _mapper.Map<AdminDto>(admin);
    }
}