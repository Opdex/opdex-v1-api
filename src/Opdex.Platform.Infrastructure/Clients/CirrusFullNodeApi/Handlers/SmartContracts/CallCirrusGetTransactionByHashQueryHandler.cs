using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;

public class CallCirrusGetTransactionByHashQueryHandler
    : IRequestHandler<CallCirrusGetTransactionByHashQuery, Transaction>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private readonly IBlockStoreModule _blockStoreModule;
    private readonly IMapper _mapper;
    private readonly ILoggerFactory _loggerFactory;

    public CallCirrusGetTransactionByHashQueryHandler(ISmartContractsModule smartContractsModule,
                                                      IBlockStoreModule blockStoreModule, IMapper mapper, ILoggerFactory loggerFactory)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public async Task<Transaction> Handle(CallCirrusGetTransactionByHashQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _smartContractsModule.GetReceiptAsync(request.TxHash, cancellationToken);
        var block = await _blockStoreModule.GetBlockAsync(transaction.BlockHash, cancellationToken);

        transaction.SetBlockHeight(block.Height);

        var transactionLogs = new List<TransactionLog>();
        for (var i = 0; i < transaction.Logs.Count; i++)
        {
            transaction.Logs[i].SortOrder = i;
            try
            {
                var log = _mapper.Map<TransactionLog>(transaction.Logs[i]);
                if (log != null) transactionLogs.Add(log);
            }
            catch (Exception ex)
            {
                // Ignored, a transaction log's name may have matched but not the schema
                var logger = _loggerFactory.CreateLogger<TransactionErrorProcessor>();
                using (logger.BeginScope(new Dictionary<string, object> { ["TxHash"] = request.TxHash }))
                {
                    logger.LogDebug(ex, "Incorrect transaction log schema in transaction receipt");
                }
            }
        }

        if (!string.IsNullOrEmpty(transaction.Error))
        {
            var errorProcessor = new TransactionErrorProcessor(_loggerFactory.CreateLogger<TransactionErrorProcessor>());
            // Todo: Capture and store errors
            _ = errorProcessor.ProcessOpdexTransactionError(transaction.Error);
        }

        return new Transaction(transaction.TransactionHash, transaction.BlockHeight, transaction.GasUsed, transaction.From,
                               transaction.To, transaction.Success, transaction.NewContractAddress, transactionLogs);
    }
}