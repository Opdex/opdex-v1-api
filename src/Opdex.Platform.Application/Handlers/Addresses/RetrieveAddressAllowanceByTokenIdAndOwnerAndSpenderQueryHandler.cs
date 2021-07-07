using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler
        : IRequestHandler<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery, AddressAllowance>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<AddressAllowance> Handle(RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(request.TokenId,
                                                                                             request.Owner,
                                                                                             request.Spender,
                                                                                             request.FindOrThrow), cancellationToken);
        }
    }
}
