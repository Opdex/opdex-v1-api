using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
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

namespace Opdex.Platform.Application.EntryHandlers.Vaults;

public class CreateVaultProposalCreateCertificateQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateVaultProposalCreateCertificateQuoteCommand>
{
    private const string MethodName = VaultConstants.Methods.CreateNewCertificateProposal;
    private readonly FixedDecimal _deposit = FixedDecimal.Parse("500.00000000");

    public CreateVaultProposalCreateCertificateQuoteCommandHandler(IMediator mediator,
                                                                   IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                   OpdexConfiguration config) : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateVaultProposalCreateCertificateQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure vault exists, if not throw to return 404
        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId), cancellationToken);

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Amount", request.Amount.ToSatoshis(token.Decimals)),
            new TransactionQuoteRequestParameter("Recipient", request.Owner),
            new TransactionQuoteRequestParameter("Description", request.Description),
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, _deposit, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
