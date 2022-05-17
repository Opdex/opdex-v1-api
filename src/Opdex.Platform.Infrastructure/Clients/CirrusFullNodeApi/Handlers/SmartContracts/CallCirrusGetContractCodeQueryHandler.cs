using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;

public class CallCirrusGetContractCodeQueryHandler : IRequestHandler<CallCirrusGetContractCodeQuery, ContractCodeDto>
{
    private readonly ISmartContractsModule _smartContractsModule;

    public CallCirrusGetContractCodeQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public Task<ContractCodeDto> Handle(CallCirrusGetContractCodeQuery request, CancellationToken cancellationToken)
    {
        return _smartContractsModule.GetContractCodeAsync(request.Contract, cancellationToken);
    }
}
