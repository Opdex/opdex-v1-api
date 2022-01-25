using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;

public class CreateCollectMiningRewardsTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCollectMiningRewardsTransactionQuoteCommand>
{
    private const string MethodName = MiningPoolConstants.Methods.CollectRewards;
    private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

    public CreateCollectMiningRewardsTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                    IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateCollectMiningRewardsTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure the mining pool exists, else throw 404 not found
        _ = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(request.MiningPool), cancellationToken);

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.MiningPool, CrsToSend, MethodName, _callbackEndpoint);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
