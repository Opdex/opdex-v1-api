using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Quotes
{
    public class CreateCreateStandardMarketTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCreateStandardMarketTransactionQuoteCommand>
    {
        private const string MethodName = MarketDeployerConstants.Methods.CreateStandardMarket;
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public CreateCreateStandardMarketTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                        IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateCreateStandardMarketTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var deployer = await _mediator.Send(new RetrieveActiveDeployerQuery(), cancellationToken);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Market Owner", new SmartContractMethodParameter(request.Owner)),
                new TransactionQuoteRequestParameter("Transaction Fee", new SmartContractMethodParameter(request.TransactionFee)),
                new TransactionQuoteRequestParameter("Authorize Providers", new SmartContractMethodParameter(request.AuthLiquidityProviders)),
                new TransactionQuoteRequestParameter("Authorize Pool Creators", new SmartContractMethodParameter(request.AuthPoolCreators)),
                new TransactionQuoteRequestParameter("Authorize Traders", new SmartContractMethodParameter(request.AuthTraders)),
                new TransactionQuoteRequestParameter("Enable Fee", new SmartContractMethodParameter(request.EnableMarketFee))
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, deployer.Address, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
