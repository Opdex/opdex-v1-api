using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Constants.SmartContracts.LiquidityPools;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes
{
    public class CreateSyncTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateSyncTransactionQuoteCommand>
    {
        private const string MethodName = LiquidityPoolConstants.Methods.Sync;
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public CreateSyncTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                        IMediator mediator, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateSyncTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure liquidity pool exists, if not throw to return 404
            _ = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool, findOrThrow: true), cancellationToken);

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.LiquidityPool, CrsToSend, MethodName, _callbackEndpoint);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
