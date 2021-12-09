using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances;

public class CreateWithdrawVaultProposalPledgeQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateWithdrawVaultProposalPledgeQuoteCommand>
{
    private const string MethodName = VaultGovernanceConstants.Methods.WithdrawPledge;

    public CreateWithdrawVaultProposalPledgeQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler, IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateWithdrawVaultProposalPledgeQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure vault and proposal exists, if not throw to return 404
        var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        _ = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.ProposalId, findOrThrow: true), cancellationToken);

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Proposal Id", request.ProposalId),
            new TransactionQuoteRequestParameter("Withdrawal Amount", (ulong)request.Amount.ToSatoshis(TokenConstants.Cirrus.Decimals)),
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Vault, FixedDecimal.Zero, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
