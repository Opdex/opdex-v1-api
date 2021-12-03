using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Transactions;

public class MakeTransactionQuoteCommandHandler : IRequestHandler<MakeTransactionQuoteCommand, TransactionQuote>
{
    private readonly IMediator _mediator;

    public MakeTransactionQuoteCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<TransactionQuote> Handle(MakeTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusLocalCallSmartContractMethodCommand(request.QuoteRequest), cancellationToken);
    }
}