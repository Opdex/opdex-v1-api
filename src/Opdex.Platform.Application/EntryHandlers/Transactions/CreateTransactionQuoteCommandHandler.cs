using MediatR;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionQuoteCommandHandler : IRequestHandler<CreateTransactionQuoteCommand, TransactionQuoteDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<TransactionQuote, TransactionQuoteDto> _quoteAssembler;

        public CreateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler, IMediator mediator)
        {
            _quoteAssembler = quoteAssembler ?? throw new ArgumentNullException(nameof(quoteAssembler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<TransactionQuoteDto> Handle(CreateTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<TransactionQuoteRequestDto>(request.QuoteRequest);
            var parameters = dto.Parameters.Select(p => new TransactionQuoteRequestParameter(p.Label, p.Value)).ToList();
            var quoteRequest = new TransactionQuoteRequest(dto.Sender, dto.To, dto.Amount, dto.Method, dto.Callback, parameters);

            var quote = await _mediator.Send(new MakeTransactionQuoteCommand(quoteRequest), cancellationToken);

            return await _quoteAssembler.Assemble(quote);
        }
    }
}
