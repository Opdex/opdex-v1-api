using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodCommandHandler : IRequestHandler<CallCirrusLocalCallSmartContractMethodCommand, TransactionQuote>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        private readonly ILoggerFactory _loggerFactory;

        public CallCirrusLocalCallSmartContractMethodCommandHandler(ISmartContractsModule smartContractsModule, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task<TransactionQuote> Handle(CallCirrusLocalCallSmartContractMethodCommand request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.QuoteRequest.To, request.QuoteRequest.Sender, request.QuoteRequest.Method,
                                                    request.QuoteRequest.SerializedParameters, amount: request.QuoteRequest.Amount);

            var response = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            string error = null;
            if (!(response.ErrorMessage?.Value is null))
            {
                var errorProcessor = new TransactionErrorProcessor(_loggerFactory.CreateLogger<TransactionErrorProcessor>());
                error = errorProcessor.ProcessOpdexTransactionError(response.ErrorMessage.Value);
            }

            var transactionLogs = response.Logs.Select(t => _mapper.Map<TransactionLog>(t)).ToList();

            return new TransactionQuote(response.Return, error, response.GasConsumed.Value, transactionLogs, request.QuoteRequest);
        }
    }
}
