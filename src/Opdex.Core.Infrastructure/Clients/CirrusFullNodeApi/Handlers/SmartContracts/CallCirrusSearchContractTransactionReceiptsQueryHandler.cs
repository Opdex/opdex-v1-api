using System;
using System.Collections.Generic;
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
    public class CallCirrusSearchContractTransactionReceiptsQueryHandler 
        : IRequestHandler<CallCirrusSearchContractTransactionReceiptsQuery, List<TransactionReceipt>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusSearchContractTransactionReceiptsQueryHandler> _logger;
        
        public CallCirrusSearchContractTransactionReceiptsQueryHandler(ISmartContractsModule smartContractsModule, 
            IMapper mapper, ILogger<CallCirrusSearchContractTransactionReceiptsQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TransactionReceipt>> Handle(CallCirrusSearchContractTransactionReceiptsQuery request, CancellationToken cancellationToken)
        {
            var transactionReceipts = new List<TransactionReceipt>();
            
            try
            {
                var transactionDtos = await _smartContractsModule.ReceiptSearchAsync(
                    request.ContractAddress, request.EventName, request.From, request.To, cancellationToken);

                foreach (var txDto in transactionDtos)
                {
                    foreach (var log in txDto.Logs)
                    {
                        if (log.Topics.Any())
                        {
                            log.Topics[0] = log.Topics[0].HexToString();
                        }
                    }

                    var transactionReceipt = _mapper.Map<TransactionReceipt>(txDto);
                    
                    transactionReceipts.Add(transactionReceipt);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching {request.EventName} event type from {request.ContractAddress}");
            }

            return transactionReceipts;
        }
    }
}