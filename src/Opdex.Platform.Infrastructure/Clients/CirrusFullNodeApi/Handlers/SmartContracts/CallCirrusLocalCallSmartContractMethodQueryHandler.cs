using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodQueryHandler : IRequestHandler<CallCirrusLocalCallSmartContractMethodQuery, LocalCallResponseDto>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusLocalCallSmartContractMethodQueryHandler> _logger;
        
        public CallCirrusLocalCallSmartContractMethodQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusLocalCallSmartContractMethodQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<LocalCallResponseDto> Handle(CallCirrusLocalCallSmartContractMethodQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, request.Method, request.Parameters);
            return await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
        }
    }
}