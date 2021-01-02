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
    public class CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler 
        : IRequestHandler<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery, IEnumerable<ReceiptDto>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler> _logger;
        
        public CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        // Todo: try catch requests
        public async Task<IEnumerable<ReceiptDto>> Handle(CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var result = await _smartContractsModule.ReceiptSearchAsync(
                request.ContractAddress, request.EventName, request.From, request.To, cancellationToken);

            return result;
        }
    }
}