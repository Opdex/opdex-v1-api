using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetTransactionReceiptByHashQueryHandler 
        : IRequestHandler<CallCirrusGetTransactionReceiptByHashQuery, TransactionReceipt>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusGetTransactionReceiptByHashQueryHandler> _logger;
        
        public CallCirrusGetTransactionReceiptByHashQueryHandler(ISmartContractsModule smartContractsModule, 
            IBlockStoreModule blockStoreModule, IMapper mapper,
            ILogger<CallCirrusGetTransactionReceiptByHashQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: try catch requests
        public async Task<TransactionReceipt> Handle(CallCirrusGetTransactionReceiptByHashQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);
            var block = await _blockStoreModule.GetBlockAsync(transaction.BlockHash, cancellationToken);

            transaction.DeserializeLogsEventType();
            transaction.SetBlockHeight(block.Height);
            
            return _mapper.Map<TransactionReceipt>(transaction);
        }
    }
}