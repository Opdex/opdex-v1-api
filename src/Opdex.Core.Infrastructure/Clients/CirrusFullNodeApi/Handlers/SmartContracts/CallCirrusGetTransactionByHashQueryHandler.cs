using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Extensions;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
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
        
        // Todo: try catch requests
        public async Task<Transaction> Handle(CallCirrusGetTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);
            var block = await _blockStoreModule.GetBlockAsync(transaction.BlockHash, cancellationToken);

            foreach (var log in transaction.Logs)
            {
                if (log.Topics.Any())
                {
                    log.Topics[0] = log.Topics[0].HexToString();
                }    
            }
            
            transaction.SetBlockHeight(block.Height);
            
            return _mapper.Map<Transaction>(transaction);
        }
    }
}