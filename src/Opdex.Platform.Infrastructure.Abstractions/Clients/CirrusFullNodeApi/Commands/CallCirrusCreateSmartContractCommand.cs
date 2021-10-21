using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCreateSmartContractCommand : IRequest<Sha256>
    {
        public CallCirrusCreateSmartContractCommand(SmartContractCreateRequestDto requestDto)
        {
            RequestDto = requestDto ?? throw new ArgumentNullException(nameof(requestDto));
        }

        public SmartContractCreateRequestDto RequestDto { get; }
    }
}
