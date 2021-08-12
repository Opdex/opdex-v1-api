using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusSearchContractTransactionsQueryHandler
        : IRequestHandler<CallCirrusSearchContractTransactionsQuery, List<Transaction>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusSearchContractTransactionsQueryHandler> _logger;

        public CallCirrusSearchContractTransactionsQueryHandler(ISmartContractsModule smartContractsModule,
            IMapper mapper, ILogger<CallCirrusSearchContractTransactionsQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Transaction>> Handle(CallCirrusSearchContractTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactionReceipts = new List<Transaction>();

            try
            {
                var transactionDtos = await _smartContractsModule.ReceiptSearchAsync(request.ContractAddress, request.LogName,
                                                                                     request.From, request.To, cancellationToken);

                foreach (var txDto in transactionDtos)
                {
                    var transactionLogs = txDto.Logs.Select(t => _mapper.Map<TransactionLog>(t)).ToList();

                    var transaction = new Transaction(txDto.TransactionHash, txDto.BlockHeight, txDto.GasUsed, txDto.From, txDto.To, txDto.Success,
                                                      txDto.NewContractAddress, transactionLogs);

                    transactionReceipts.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching {request.LogName} log type from {request.ContractAddress}");
            }

            return transactionReceipts;
        }
    }
}
