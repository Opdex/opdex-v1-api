using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningGovernances;

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
        // ensure mining governance contract exists, throw 404 if not
        _ = await _mediator.Send(new RetrieveMiningGovernanceByAddressQuery(request.MiningGovernance), cancellationToken);

        var methodName = request.FullDistribution ? MiningGovernanceConstants.Methods.RewardMiningPools : MiningGovernanceConstants.Methods.RewardMiningPool;

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.MiningGovernance, AmountCrs, methodName, _callbackEndpoint);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
