using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressBalanceByTokenIdAndOwnerQueryHandler
        : IRequestHandler<RetrieveAddressBalanceByTokenIdAndOwnerQuery, AddressBalance>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressBalanceByTokenIdAndOwnerQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<AddressBalance> Handle(RetrieveAddressBalanceByTokenIdAndOwnerQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAddressBalanceByTokenIdAndOwnerQuery(request.TokenId, request.Owner), cancellationToken);
        }
    }
}