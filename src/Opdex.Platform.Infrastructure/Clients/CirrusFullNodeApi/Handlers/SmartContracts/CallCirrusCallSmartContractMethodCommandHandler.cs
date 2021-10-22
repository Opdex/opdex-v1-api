using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusCallSmartContractMethodCommandHandler : IRequestHandler<CallCirrusCallSmartContractMethodCommand, Sha256>
    {
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusCallSmartContractMethodCommandHandler(ISmartContractsModule smartContractsModule)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public Task<Sha256> Handle(CallCirrusCallSmartContractMethodCommand request, CancellationToken cancellationToken)
        {
            SmartContractCallRequestDto callRequest;

            if (request.QuoteRequest == null)
            {
                callRequest = request.CallDto;
            }
            else
            {
                var parameters = request.QuoteRequest.SerializedParameters;

                callRequest = new SmartContractCallRequestDto(request.QuoteRequest.To, "cirrusdev", request.QuoteRequest.Sender,
                                                              "password", request.QuoteRequest.Amount, request.QuoteRequest.Method, parameters);

            }

            return _smartContractsModule.CallSmartContractAsync(callRequest, cancellationToken);
        }
    }
}
