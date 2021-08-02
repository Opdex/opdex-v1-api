using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class MakeTokenCommandHandler : IRequestHandler<MakeTokenCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeTokenCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(MakeTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenSummaryQuery = new CallCirrusGetSrcTokenDetailsByAddressQuery(request.Address);

            var token = request.Token ?? await _mediator.Send(tokenSummaryQuery, cancellationToken);

            return await _mediator.Send(new PersistTokenCommand(token), cancellationToken);
        }
    }
}
