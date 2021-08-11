using MediatR;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionBroadcastCommandHandler
        : IRequestHandler<CreateTransactionBroadcastCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateTransactionBroadcastCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(CreateTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<TransactionQuoteRequestDto>(request.QuoteRequest);
            var parameters = dto.Parameters.Select(p => new TransactionQuoteRequestParameter(p.Label, p.Value)).ToList();
            var quoteRequest = new TransactionQuoteRequest(dto.Sender, dto.To, dto.Amount, dto.Method, dto.Callback, parameters);

            return _mediator.Send(new MakeTransactionBroadcastCommand(quoteRequest), cancellationToken);
        }
    }
}
