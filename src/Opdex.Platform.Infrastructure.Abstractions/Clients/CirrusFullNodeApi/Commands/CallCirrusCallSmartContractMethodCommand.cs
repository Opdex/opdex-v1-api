using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCallSmartContractMethodCommand : IRequest<string>
    {
        public CallCirrusCallSmartContractMethodCommand(SmartContractCallRequestDto call)
        {
            CallDto = call;
        }
        
        public SmartContractCallRequestDto CallDto { get; }
    }
}