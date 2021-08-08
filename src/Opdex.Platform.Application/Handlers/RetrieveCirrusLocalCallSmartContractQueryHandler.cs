using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Platform.Application.Handlers
{
    public class RetrieveCirrusLocalCallSmartContractQueryHandler : IRequestHandler<RetrieveCirrusLocalCallSmartContractQuery, LocalCallResponseDto>
    {
        private readonly IMediator _mediator;
        
        public RetrieveCirrusLocalCallSmartContractQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<LocalCallResponseDto> Handle(RetrieveCirrusLocalCallSmartContractQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusLocalCallSmartContractMethodQuery(request.Address, request.Method, request.Parameters), cancellationToken);
        }
    }
}