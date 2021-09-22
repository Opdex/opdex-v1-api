using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes
{
    public class CreateStartMiningTransactionQuoteCommandHandler
        : BaseTransactionQuoteCommandHandler<CreateStartMiningTransactionQuoteCommand>
    {
        private const string MethodName = MiningPoolConstants.Methods.StartMining;
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public CreateStartMiningTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                               IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateStartMiningTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure the mining pool exists, else throw 404 not found
            _ = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.MiningPool), cancellationToken);

            var amount = request.Amount.ToSatoshis(TokenConstants.LiquidityPoolToken.Decimals);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", amount)
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.MiningPool, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
