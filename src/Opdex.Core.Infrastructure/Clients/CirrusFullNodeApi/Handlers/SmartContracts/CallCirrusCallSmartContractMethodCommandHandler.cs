using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts
{
    public class CallCirrusCallSmartContractMethodCommandHandler : IRequestHandler<CallCirrusCallSmartContractMethodCommand, string>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly IMapper _mapper;
        
        public CallCirrusCallSmartContractMethodCommandHandler(ISmartContractsModule smartContractsModule, IMapper mapper)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }
        
        public Task<string> Handle(CallCirrusCallSmartContractMethodCommand request, CancellationToken cancellationToken)
        {
            return _smartContractsModule.CallSmartContractAsync(request.CallDto, cancellationToken);
        }
    }
}