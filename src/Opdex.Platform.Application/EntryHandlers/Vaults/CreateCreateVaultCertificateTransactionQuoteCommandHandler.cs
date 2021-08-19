using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateCreateVaultCertificateTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCreateVaultCertificateTransactionQuoteCommand>
    {
        private const string MethodName = VaultConstants.Methods.CreateCertificate;
        private const string CrsToSend = "0";

        public CreateCreateVaultCertificateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                    IMediator mediator, OpdexConfiguration config)
            : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateCreateVaultCertificateTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            var amount = UInt256.Parse(request.Amount.ToSatoshis(TokenConstants.Opdex.Decimals));

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Holder", request.Holder),
                new TransactionQuoteRequestParameter("Amount", amount)
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.ContractAddress, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}