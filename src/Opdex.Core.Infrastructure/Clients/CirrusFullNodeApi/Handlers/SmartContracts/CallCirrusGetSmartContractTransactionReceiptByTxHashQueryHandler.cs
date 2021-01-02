using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler 
        : IRequestHandler<CallCirrusGetSmartContractTransactionReceiptByTxHashQuery, ReceiptDto>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler> _logger;
        
        public CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        // Todo: try catch requests
        public async Task<ReceiptDto> Handle(CallCirrusGetSmartContractTransactionReceiptByTxHashQuery request, CancellationToken cancellationToken)
        {
            var result = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);

            return result;
        }
    }
}