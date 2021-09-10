using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes
{
    public class CreateAddLiquidityTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateAddLiquidityTransactionQuoteCommand>
    {
        private const string MethodName = RouterConstants.Methods.AddLiquidity;

        public CreateAddLiquidityTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                IMediator mediator, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateAddLiquidityTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.LiquidityPool), cancellationToken);
            var token = await _mediator.Send(new RetrieveTokenByIdQuery(pool.SrcTokenId), cancellationToken);
            var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(pool.MarketId), cancellationToken);

            var amountSrc = request.AmountSrc.ToSatoshis(token.Decimals);
            var amountCrsMin = request.AmountCrsMin.ToSatoshis(TokenConstants.Cirrus.Decimals);
            var amountSrcMin = request.AmountSrcMin.ToSatoshis(token.Decimals);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Token", token.Address),
                new TransactionQuoteRequestParameter("SRC Amount", amountSrc),
                new TransactionQuoteRequestParameter("Minimum CRS Amount", (ulong)amountCrsMin),
                new TransactionQuoteRequestParameter("Minimum SRC Amount", amountSrcMin),
                new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                new TransactionQuoteRequestParameter("Deadline", request.Deadline)
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, router.Address, request.AmountCrs, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
