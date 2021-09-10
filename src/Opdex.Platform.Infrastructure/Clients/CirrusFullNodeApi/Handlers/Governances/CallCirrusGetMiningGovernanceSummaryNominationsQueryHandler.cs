using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Governances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Governances
{
    public class CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler
        : IRequestHandler<CallCirrusGetMiningGovernanceSummaryNominationsQuery, IEnumerable<MiningGovernanceNominationCirrusDto>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler> _logger;

        public CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusGetMiningGovernanceSummaryNominationsQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MiningGovernanceNominationCirrusDto>> Handle(CallCirrusGetMiningGovernanceSummaryNominationsQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Nominations", new string[0], request.BlockHeight);
            var nominationsResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);

            var serialized = JsonConvert.SerializeObject(nominationsResponse.Return);
            var nominations = JsonConvert.DeserializeObject<IEnumerable<MiningGovernanceNominationCirrusDto>>(serialized);

            return nominations;
        }
    }
}
