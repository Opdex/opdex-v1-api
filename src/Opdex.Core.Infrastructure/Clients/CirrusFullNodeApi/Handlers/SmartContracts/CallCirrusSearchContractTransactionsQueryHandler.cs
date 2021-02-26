using System;
using System.Collections.Generic;
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
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Transaction>> Handle(CallCirrusSearchContractTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactionReceipts = new List<Transaction>();
            
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

                    var transactionReceipt = _mapper.Map<Transaction>(txDto);
                    
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