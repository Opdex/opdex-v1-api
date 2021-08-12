using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionQuoteCommandHandler : IRequestHandler<CreateTransactionQuoteCommand, TransactionQuoteDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<TransactionQuote, TransactionQuoteDto> _quoteAssembler;

        public CreateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                    IMediator mediator, IMapper mapper)
        {
            _quoteAssembler = quoteAssembler ?? throw new ArgumentNullException(nameof(quoteAssembler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransactionQuoteDto> Handle(CreateTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<TransactionQuoteRequestDto>(request.QuoteRequest);

            var quoteRequest = _mapper.Map<TransactionQuoteRequest>(dto);

            var quote = await _mediator.Send(new MakeTransactionQuoteCommand(quoteRequest), cancellationToken);

            return await _quoteAssembler.Assemble(quote);
        }
    }
}
