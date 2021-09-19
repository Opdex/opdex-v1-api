using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Quotes
{
    public class CreateRevokeVaultCertificatesTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateRevokeVaultCertificatesTransactionQuoteCommand>
    {
        private const string MethodName = VaultConstants.Methods.RevokeCertificates;
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public CreateRevokeVaultCertificatesTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                           IMediator mediator,
                                                                           OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateRevokeVaultCertificatesTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure vault exists, if not throw to return 404
            _ = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Holder", request.Holder)
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
