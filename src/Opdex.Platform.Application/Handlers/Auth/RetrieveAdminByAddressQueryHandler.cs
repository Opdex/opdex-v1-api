using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Auth;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Auth;

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
