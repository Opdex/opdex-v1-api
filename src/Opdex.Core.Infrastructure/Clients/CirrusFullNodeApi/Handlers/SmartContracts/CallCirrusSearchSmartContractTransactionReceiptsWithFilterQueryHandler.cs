using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler 
        : IRequestHandler<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery, List<TransactionReceipt>>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        private readonly ILogger<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler> _logger;
        
        public CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler(ISmartContractsModule smartContractsModule, 
            IMapper mapper, ILogger<CallCirrusSearchSmartContractTransactionReceiptsWithFilterQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<TransactionReceipt>> Handle(CallCirrusSearchSmartContractTransactionReceiptsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var transactionDtos = Enumerable.Empty<TransactionReceiptDto>();

            try
            {
                transactionDtos = await _smartContractsModule.ReceiptSearchAsync(
                    request.ContractAddress, request.EventName, request.From, request.To, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching {request.EventName} event type from {request.ContractAddress}");
            }

            return transactionDtos
                .Select(tx => _mapper.Map<TransactionReceipt>(tx))
                .ToList();
        }
    }
}