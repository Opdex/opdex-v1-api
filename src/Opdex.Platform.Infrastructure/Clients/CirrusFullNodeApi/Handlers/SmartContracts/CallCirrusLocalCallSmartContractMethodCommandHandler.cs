using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodCommandHandler : IRequestHandler<CallCirrusLocalCallSmartContractMethodCommand, TransactionQuote>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusLocalCallSmartContractMethodCommandHandler> _logger;
        private readonly IMapper _mapper;

        public CallCirrusLocalCallSmartContractMethodCommandHandler(ISmartContractsModule smartContractsModule,
            ILogger<CallCirrusLocalCallSmartContractMethodCommandHandler> logger, IMapper mapper)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TransactionQuote> Handle(CallCirrusLocalCallSmartContractMethodCommand request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.QuoteRequest.To, request.QuoteRequest.Sender,
                                                    request.QuoteRequest.Method, request.QuoteRequest.SerializedParameters);

            var response = await _smartContractsModule.LocalCallAsync(localCall, cancellationToken);

            var transactionLogs = response.Logs.Select(t => _mapper.Map<TransactionLog>(t)).ToList();

            // Todo: Better handle and parse errors into friendly Enum => Constant messages
            return new TransactionQuote(response.Return, response.ErrorMessage?.Value, response.GasConsumed.Value, transactionLogs, request.QuoteRequest);
        }
    }
}
