using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses.Balances
{
    public class RetrieveAddressBalancesByModifiedBlockQueryHandler: IRequestHandler<RetrieveAddressBalancesByModifiedBlockQuery, IEnumerable<AddressBalance>>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressBalancesByModifiedBlockQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<AddressBalance>> Handle(RetrieveAddressBalancesByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAddressBalancesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
        }
    }
}
