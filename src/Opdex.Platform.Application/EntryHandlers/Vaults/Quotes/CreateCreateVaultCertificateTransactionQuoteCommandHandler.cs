using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Quotes;

public class CreateCreateVaultCertificateTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCreateVaultCertificateTransactionQuoteCommand>
{
    private const string MethodName = VaultConstants.Methods.CreateCertificate;
    private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

    public CreateCreateVaultCertificateTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                      IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateCreateVaultCertificateTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure the vault exists, else throw 404 not found
        _ = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault), cancellationToken);

        UInt256 amount;
        try
        {
            amount = request.Amount.ToSatoshis(TokenConstants.Opdex.Decimals);
        }
        catch (OverflowException)
        {
            throw new InvalidDataException("amount", "Value too large.");
        }

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Holder", request.Holder),
            new TransactionQuoteRequestParameter("Amount", amount)
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);

    }
}