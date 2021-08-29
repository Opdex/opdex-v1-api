using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets
{
    public class CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateClaimStandardMarketOwnershipTransactionQuoteCommand>
    {
        private const string MethodName = StandardMarketConstants.Methods.ClaimPendingOwnership;
        private const string CrsToSend = "0";

        public CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                                IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateClaimStandardMarketOwnershipTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure market exists, if not throw to return 404
            _ = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Market, CrsToSend, MethodName, _callbackEndpoint);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}