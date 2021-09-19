using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Governances
{
    public class CallCirrusGetGovernanceNominationsSummaryQueryHandler
        : IRequestHandler<CallCirrusGetGovernanceNominationsSummaryQuery, IEnumerable<GovernanceContractNominationSummary>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly string _methodName = $"get_{GovernanceConstants.Properties.Nominations}";

        public CallCirrusGetGovernanceNominationsSummaryQueryHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<IEnumerable<GovernanceContractNominationSummary>> Handle(CallCirrusGetGovernanceNominationsSummaryQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Governance, request.Governance, _methodName, request.BlockHeight);

            // Have to use a local call here rather than get storage property.
            // Local call can deserialize the struct[] response, get storage property would return bytecode in which
            // we would have to add a Stratis Serialization dependency, currently only available in netcoreapp projects.
            var nominationsResponse = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            var serialized = JsonConvert.SerializeObject(nominationsResponse.Return);

            return JsonConvert.DeserializeObject<IEnumerable<GovernanceContractNominationSummary>>(serialized);
        }
    }
}
