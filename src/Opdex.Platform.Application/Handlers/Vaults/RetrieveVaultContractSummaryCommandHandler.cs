using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveVaultContractSummaryCommandHandler
        : IRequestHandler<RetrieveVaultContractSummaryCommand, VaultContractSummary>
    {
        private readonly IMediator _mediator;

        public RetrieveVaultContractSummaryCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<VaultContractSummary> Handle(RetrieveVaultContractSummaryCommand request, CancellationToken cancellationToken)
        {
            var lockedToken = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                               VaultConstants.StateKeys.Token,
                                                                                               SmartContractParameterType.Address,
                                                                                               request.BlockHeight));

            var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                         VaultConstants.StateKeys.Owner,
                                                                                         SmartContractParameterType.Address,
                                                                                         request.BlockHeight));

            var supply = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                          VaultConstants.StateKeys.TotalSupply,
                                                                                          SmartContractParameterType.UInt256,
                                                                                          request.BlockHeight));

            var gensis = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Vault,
                                                                                          VaultConstants.StateKeys.Genesis,
                                                                                          SmartContractParameterType.UInt64,
                                                                                          request.BlockHeight));

            return new VaultContractSummary(lockedToken.Parse<Address>(), gensis.Parse<ulong>(),
                                            owner.Parse<Address>(), supply.Parse<UInt256>());
        }
    }
}
