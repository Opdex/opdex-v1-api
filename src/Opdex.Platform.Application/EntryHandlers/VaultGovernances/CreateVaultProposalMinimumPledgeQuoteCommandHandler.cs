using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
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

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class CreateVaultProposalMinimumPledgeQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateVaultProposalMinimumPledgeQuoteCommand>
{
    private const string MethodName = VaultGovernanceConstants.Methods.CreateTotalPledgeMinimumProposal;
    private readonly FixedDecimal _deposit = FixedDecimal.Parse("500.00000000");

    public CreateVaultProposalMinimumPledgeQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                               IMediator mediator,
                                                               OpdexConfiguration config) : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateVaultProposalMinimumPledgeQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure vault exists, if not throw to return 404
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId), cancellationToken);

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Amount", request.Amount.ToSatoshis(token.Decimals)),
            new TransactionQuoteRequestParameter("Description", request.Description),
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, _deposit, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
