using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetSmartContractReceiptWithFilterQueryHandler : IRequestHandler<CallCirrusGetSmartContractReceiptWithFilterQuery, IEnumerable<ReceiptDto>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSmartContractReceiptWithFilterQueryHandler> _logger;
        
        public CallCirrusGetSmartContractReceiptWithFilterQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSmartContractReceiptWithFilterQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        public async Task<IEnumerable<ReceiptDto>> Handle(CallCirrusGetSmartContractReceiptWithFilterQuery request, CancellationToken cancellationToken)
        {
            var result = await _smartContractsModule.ReceiptSearchAsync(
                request.ContractAddress, request.EventName, request.From, request.To, cancellationToken);

            return result;
        }
    }
}