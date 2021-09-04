using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets
{
    public class CreateCreateStakingMarketTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCreateStakingMarketTransactionQuoteCommand>
    {
        private const string MethodName = MarketDeployerConstants.Methods.CreateStakingMarket;
        private const string CrsToSend = "0";

        public CreateCreateStakingMarketTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                       IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateCreateStakingMarketTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var deployer = await _mediator.Send(new RetrieveActiveDeployerQuery(), cancellationToken);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Staking Token", new SmartContractMethodParameter(request.StakingToken))
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, deployer.Address, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
