using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionBroadcastCommandHandler
        : IRequestHandler<CreateTransactionBroadcastCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateTransactionBroadcastCommandHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<string> Handle(CreateTransactionBroadcastCommand request, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<TransactionQuoteRequestDto>(request.QuoteRequest);

            var quoteRequest = _mapper.Map<TransactionQuoteRequest>(dto);

            return _mediator.Send(new MakeTransactionBroadcastCommand(quoteRequest), cancellationToken);
        }
    }
}
