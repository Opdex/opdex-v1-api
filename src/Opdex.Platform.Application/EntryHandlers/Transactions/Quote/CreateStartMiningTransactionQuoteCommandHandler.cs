using MediatR;
using Opdex.Platform.Application.Abstractions.Commands;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Quote;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
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
        private readonly IModelAssembler<TransactionQuote, TransactionQuoteDto> _quoteAssembler;
        private readonly string _callbackEndpoint;

        private const string MethodName = MiningPoolConstants.Methods.StartMining;
        private const string CrsToSend = "0";

        public CreateStartMiningTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                               IMediator mediator, OpdexConfiguration configuration)
        {
            _quoteAssembler = quoteAssembler ?? throw new ArgumentNullException(nameof(quoteAssembler));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _callbackEndpoint = configuration.WalletTransactionCallback ?? throw new ArgumentNullException(nameof(configuration.WalletTransactionCallback));
        }

        public async Task<TransactionQuoteDto> Handle(CreateStartMiningTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var amount = request.Amount.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", amount, SmartContractParameterType.UInt256)
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.MiningPool, CrsToSend, MethodName,
                                                           _callbackEndpoint, requestParameters);

            var quote = await _mediator.Send(new MakeTransactionQuoteCommand(quoteRequest), cancellationToken);

            return await _quoteAssembler.Assemble(quote);
        }
    }
}
