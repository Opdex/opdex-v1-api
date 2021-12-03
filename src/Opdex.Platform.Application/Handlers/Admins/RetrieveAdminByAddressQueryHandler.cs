using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Admins;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Admins;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Admins;

public class RetrieveAdminByAddressQueryHandler : IRequestHandler<RetrieveAdminByAddressQuery, Admin>
{
    private readonly IMediator _mediator;

    public RetrieveAdminByAddressQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<Admin> Handle(RetrieveAdminByAddressQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectAdminByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
    }
}