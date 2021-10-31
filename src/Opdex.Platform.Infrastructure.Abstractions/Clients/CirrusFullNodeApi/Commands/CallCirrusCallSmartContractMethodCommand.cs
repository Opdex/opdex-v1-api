using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    public class CallCirrusCallSmartContractMethodCommand : IRequest<Sha256>
    {
        public CallCirrusCallSmartContractMethodCommand(SmartContractCallRequestDto callDto)
        {
            if (callDto is null) throw new ArgumentNullException(nameof(callDto), "Call must not be null.");

            CallDto = callDto;
        }

        public SmartContractCallRequestDto CallDto { get; }
    }
}
