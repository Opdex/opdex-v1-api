using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateRedeemVaultCertificatesTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateRedeemVaultCertificatesTransactionQuoteCommand>
    {
        private const string MethodName = VaultConstants.Methods.RedeemCertificates;
        private const string CrsToSend = "0";

        public CreateRedeemVaultCertificatesTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                           IMediator mediator,
                                                                           OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateRedeemVaultCertificatesTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure vault exists, if not throw to return 404
            _ = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, CrsToSend, MethodName, _callbackEndpoint);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
