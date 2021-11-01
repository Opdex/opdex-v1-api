using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class CreateTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateTransactionQuoteCommand>
    {
        private readonly IMapper _mapper;

        public CreateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                    IMediator mediator, IMapper mapper, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<TransactionQuoteDto> Handle(CreateTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var dto = JsonConvert.DeserializeObject<TransactionQuoteRequestDto>(request.QuoteRequest);

            var quoteRequest = _mapper.Map<TransactionQuoteRequest>(dto);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
