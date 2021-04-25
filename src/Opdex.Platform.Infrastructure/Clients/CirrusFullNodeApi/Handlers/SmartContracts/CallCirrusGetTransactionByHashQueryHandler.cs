using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Extensions;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusGetTransactionByHashQueryHandler 
        : IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IBlockStoreModule _blockStoreModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusGetTransactionByHashQueryHandler> _logger;
        
        public CallCirrusGetTransactionByHashQueryHandler(ISmartContractsModule smartContractsModule, 
            IBlockStoreModule blockStoreModule, IMapper mapper,
            ILogger<CallCirrusGetTransactionByHashQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: try catch with retry external from polly for 400 Bad Requests based on flag
        // (incase transaction has not been included in block yet)
        public async Task<Transaction> Handle(CallCirrusGetTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);
            var block = await _blockStoreModule.GetBlockAsync(transaction.BlockHash, cancellationToken);

            transaction.SetBlockHeight(block.Height);

            var transactionResponse =  _mapper.Map<Transaction>(transaction);

            for (var i = 0; i < transaction.Logs.Length; i++)
            {
                var txLog = transaction.Logs[i];
                var logType = txLog.Topics[0].HexToString();
                
                transactionResponse.DeserializeLog(txLog.Address, logType, i, txLog.Log);
            }

            return transactionResponse;
        }
    }
}