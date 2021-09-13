using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Governances
{
    public class CreateRewardMiningPoolsTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateRewardMiningPoolsTransactionQuoteCommand>
    {
        private static readonly FixedDecimal AmountCrs = FixedDecimal.Zero;

        public CreateRewardMiningPoolsTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                     IMediator mediator, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateRewardMiningPoolsTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure governance contract exists, throw 404 if not
            _ = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.Governance), cancellationToken);

            var methodName = request.FullDistribution ? GovernanceConstants.Methods.RewardMiningPools : GovernanceConstants.Methods.RewardMiningPool;

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Governance, AmountCrs, methodName, _callbackEndpoint);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
