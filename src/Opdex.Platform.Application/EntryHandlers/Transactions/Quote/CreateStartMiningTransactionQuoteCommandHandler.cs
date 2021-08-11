using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Quote;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Quote
{
    public class CreateStartMiningTransactionQuoteCommandHandler
        : IRequestHandler<CreateStartMiningTransactionQuoteCommand, TransactionQuoteDto>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> _eventsAssembler;
        private readonly string _callbackEndpoint;

        private const string MethodName = MiningPoolConstants.Methods.StartMining;
        private const string CrsToSend = "0";

        public CreateStartMiningTransactionQuoteCommandHandler(IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>> eventsAssembler,
                                                               IMediator mediator, IMapper mapper, OpdexConfiguration configuration)
        {
            _eventsAssembler = eventsAssembler ?? throw new ArgumentNullException(nameof(eventsAssembler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _callbackEndpoint = configuration?.WalletTransactionCallback.HasValue() == true
                ? configuration.WalletTransactionCallback
                : throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransactionQuoteDto> Handle(CreateStartMiningTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var amount = request.Amount.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", amount, SmartContractParameterType.UInt256)
            };

            // Todo: Using request.MiningPool, see blow Todo comment relating to using our DB in quotes. Not checking DB this is a valid MiningPool currently
            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.MiningPool, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            var quote = await _mediator.Send(new RetrieveCirrusLocalCallSmartContractQuery(quoteRequest), cancellationToken);

            var transactionQuote = _mapper.Map<TransactionQuoteDto>(quote);

            // Todo: This is shard so we can show events in rich context, however, we may want to quote users exclusively using Cirrus FN
            transactionQuote.Events = await _eventsAssembler.Assemble(quote.Logs);

            return transactionQuote;
        }
    }
}
