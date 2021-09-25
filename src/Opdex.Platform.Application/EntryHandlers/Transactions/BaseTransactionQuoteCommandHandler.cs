using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public abstract class BaseTransactionQuoteCommandHandler<TCommand> : IRequestHandler<TCommand, TransactionQuoteDto>
        where TCommand : IRequest<TransactionQuoteDto>
    {
        protected readonly IMediator _mediator;
        protected readonly string _callbackEndpoint;
        private readonly IModelAssembler<TransactionQuote, TransactionQuoteDto> _quoteAssembler;

        protected BaseTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler, IMediator mediator, OpdexConfiguration config)
        {
            _quoteAssembler = quoteAssembler ?? throw new ArgumentNullException(nameof(quoteAssembler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _callbackEndpoint = config.WalletTransactionCallback ?? throw new ArgumentNullException(nameof(config.WalletTransactionCallback));
        }

        public abstract Task<TransactionQuoteDto> Handle(TCommand request, CancellationToken cancellationToken);

        protected async Task<TransactionQuoteDto> ExecuteAsync(TransactionQuoteRequest quoteRequest, CancellationToken cancellationToken)
        {
            var quote = await _mediator.Send(new MakeTransactionQuoteCommand(quoteRequest), cancellationToken);

            return await _quoteAssembler.Assemble(quote);
        }
    }
}
