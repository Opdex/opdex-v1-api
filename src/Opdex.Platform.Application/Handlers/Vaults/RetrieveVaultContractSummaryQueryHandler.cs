using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultContractSummaryQueryHandler : IRequestHandler<RetrieveVaultContractSummaryQuery, VaultContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultContractSummaryQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<VaultContractSummary> Handle(RetrieveVaultContractSummaryQuery request, CancellationToken cancellationToken)
        {
            var summary = new VaultContractSummary(request.BlockHeight);

            if (request.IncludeLockedToken)
            {
                var lockedToken = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                                   VaultConstants.StateKeys.Token,
                                                                                                   SmartContractParameterType.Address,
                                                                                                   request.BlockHeight));

                summary.SetLockedToken(lockedToken);
            }

            if (request.IncludeOwner)
            {
                var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                             VaultConstants.StateKeys.Owner,
                                                                                             SmartContractParameterType.Address,
                                                                                             request.BlockHeight));

                summary.SetOwner(owner);
            }

            if (request.IncludeSupply)
            {
                var supply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                              VaultConstants.StateKeys.TotalSupply,
                                                                                              SmartContractParameterType.UInt256,
                                                                                              request.BlockHeight));

                summary.SetUnassignedSupply(supply);
            }

            if (request.IncludeGenesis)
            {
                var genesis = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                               VaultConstants.StateKeys.Genesis,
                                                                                               SmartContractParameterType.UInt64,
                                                                                               request.BlockHeight));

                summary.SetGenesis(genesis);
            }

            return summary;
        }
    }
}
