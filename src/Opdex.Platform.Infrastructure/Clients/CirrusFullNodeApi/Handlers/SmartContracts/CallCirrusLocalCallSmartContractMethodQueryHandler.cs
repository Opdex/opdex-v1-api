using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodQueryHandler : IRequestHandler<CallCirrusLocalCallSmartContractMethodQuery, TransactionQuote>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusLocalCallSmartContractMethodQueryHandler> _logger;
        private readonly IMapper _mapper;

        public CallCirrusLocalCallSmartContractMethodQueryHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusLocalCallSmartContractMethodQueryHandler> logger, IMapper mapper)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransactionQuote> Handle(CallCirrusLocalCallSmartContractMethodQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.QuoteRequest.To, request.QuoteRequest.Sender,
                                                    request.QuoteRequest.Method, request.QuoteRequest.SerializedParameters);

            var response = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);

            var transactionLogs = response.Logs.Select(t => _mapper.Map<TransactionLog>(t)).ToList();

            return new TransactionQuote(response.Return, response.ErrorMessage, response.GasConsumed.Value, transactionLogs, request.QuoteRequest);
        }
    }
}
