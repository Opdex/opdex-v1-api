using System;
using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCreateSmartContractCommand : IRequest<string>
    {
        public CallCirrusCreateSmartContractCommand(SmartContractCreateRequestDto requestDto)
        {
            RequestDto = requestDto ?? throw new ArgumentNullException(nameof(requestDto));
        }
        
        public SmartContractCreateRequestDto RequestDto { get; }
    }
}