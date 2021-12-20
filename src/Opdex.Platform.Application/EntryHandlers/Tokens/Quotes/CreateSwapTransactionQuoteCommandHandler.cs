using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;

public class CreateSwapTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateSwapTransactionQuoteCommand>
{
    public CreateSwapTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                    IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateSwapTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        var tokenIn = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenIn), cancellationToken);
        var tokenOut = await _mediator.Send(new RetrieveTokenByAddressQuery(request.TokenOut), cancellationToken);
        var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market), cancellationToken);
        var router = await _mediator.Send(new RetrieveActiveMarketRouterByMarketIdQuery(market.Id), cancellationToken);

        var isCrsOut = tokenOut.Address == Address.Cirrus;
        var isCrsIn = tokenIn.Address == Address.Cirrus;
        var crsToSend = isCrsIn
            ? request.TokenInExactAmount
                ? request.TokenInAmount
                : request.TokenInMaximumAmount
            : FixedDecimal.Zero;
        string methodName;
        List<TransactionQuoteRequestParameter> parameters;

        if (isCrsOut)
        {
            if (request.TokenInExactAmount)
            {
                methodName = RouterConstants.Methods.SwapExactSrcForCrs;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Amount In", request.TokenInAmount.ToSatoshis(tokenIn.Decimals)),
                    new TransactionQuoteRequestParameter("Minimum Amount Out", (ulong)request.TokenOutMinimumAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
            else
            {
                methodName = RouterConstants.Methods.SwapSrcForExactCrs;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Amount Out", (ulong)request.TokenOutAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Maximum Amount In", request.TokenInMaximumAmount.ToSatoshis(tokenIn.Decimals)),
                    new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
        }
        else if (isCrsIn)
        {
            if (request.TokenInExactAmount)
            {
                methodName = RouterConstants.Methods.SwapExactCrsForSrc;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Minimum Amount Out", request.TokenOutMinimumAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
            else
            {
                methodName = RouterConstants.Methods.SwapCrsForExactSrc;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Amount Out", request.TokenOutAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
        }
        else
        {
            if (request.TokenInExactAmount)
            {
                methodName = RouterConstants.Methods.SwapExactSrcForSrc;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Amount In", request.TokenInAmount.ToSatoshis(tokenIn.Decimals)),
                    new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                    new TransactionQuoteRequestParameter("Minimum Amount Out", request.TokenOutMinimumAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
            else
            {
                methodName = RouterConstants.Methods.SwapSrcForExactSrc;
                parameters = new List<TransactionQuoteRequestParameter>
                {
                    new TransactionQuoteRequestParameter("Maximum Amount In", request.TokenInMaximumAmount.ToSatoshis(tokenIn.Decimals)),
                    new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                    new TransactionQuoteRequestParameter("Amount Out", request.TokenOutAmount.ToSatoshis(tokenOut.Decimals)),
                    new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                    new TransactionQuoteRequestParameter("Recipient", request.Recipient),
                    new TransactionQuoteRequestParameter("Deadline", request.Deadline)
                };
            }
        }

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, router.Address, crsToSend, methodName, _callbackEndpoint, parameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
