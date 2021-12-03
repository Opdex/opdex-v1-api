using MediatR;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.MiningGovernances;

public class CallCirrusGetMiningGovernanceNominationsSummaryQueryHandler
    : IRequestHandler<CallCirrusGetMiningGovernanceNominationsSummaryQuery, IEnumerable<MiningGovernanceContractNominationSummary>>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private readonly string _methodName = $"get_{MiningGovernanceConstants.Properties.Nominations}";

    public CallCirrusGetMiningGovernanceNominationsSummaryQueryHandler(ISmartContractsModule smartContractsModule)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
    }

    public async Task<IEnumerable<MiningGovernanceContractNominationSummary>> Handle(CallCirrusGetMiningGovernanceNominationsSummaryQuery request, CancellationToken cancellationToken)
    {
        var localCall = new LocalCallRequestDto(request.MiningGovernance, request.MiningGovernance, _methodName, request.BlockHeight);

        // Have to use a local call here rather than get storage property.
        // Local call can deserialize the struct[] response, get storage property would return bytecode in which
        // we would have to add a Stratis Serialization dependency, currently only available in netcoreapp projects.
        var nominationsResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

        return nominationsResponse.DeserializeValue<IEnumerable<MiningGovernanceContractNominationSummary>>();
    }
}