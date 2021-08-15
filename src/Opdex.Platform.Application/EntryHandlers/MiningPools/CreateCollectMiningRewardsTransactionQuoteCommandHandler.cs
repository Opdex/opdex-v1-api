using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools
{
    public class CreateCollectMiningRewardsTransactionQuoteCommandHandler
        : BaseQuoteCommandHandler<CreateCollectMiningRewardsTransactionQuoteCommand>
    {
        private const string MethodName = MiningPoolConstants.Methods.CollectRewards;
        private const string CrsToSend = "0";

        public CreateCollectMiningRewardsTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                        IMediator mediator, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateCollectMiningRewardsTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.ContractAddress, CrsToSend, MethodName, _callbackEndpoint);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
