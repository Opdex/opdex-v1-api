using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler 
        : IRequestHandler<CallCirrusGetSmartContractTransactionReceiptByTxHashQuery, TransactionReceipt>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler> _logger;
        
        public CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler(ISmartContractsModule smartContractsModule, 
            IMapper mapper,
            ILogger<CallCirrusGetSmartContractTransactionReceiptByTxHashQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: try catch requests
        public async Task<TransactionReceipt> Handle(CallCirrusGetSmartContractTransactionReceiptByTxHashQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);
            
            return _mapper.Map<TransactionReceipt>(transaction);
        }
    }
}