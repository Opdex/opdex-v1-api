using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands
{
    /// <summary>
    /// Generic command to call a smart contract and broadcast the transaction. Intended only for contract deployment.
    /// </summary>
    public class CallCirrusCallSmartContractMethodCommand : IRequest<Sha256>
    {
        /// <summary>
        /// Creates a command to call a smart contract and broadcast the transaction.
        /// </summary>
        /// <param name="callDto">The parameters of the call smart contract request.</param>
        public CallCirrusCallSmartContractMethodCommand(SmartContractCallRequestDto callDto)
        {
            if (callDto is null) throw new ArgumentNullException(nameof(callDto), "Call must not be null.");

            CallDto = callDto;
        }

        public SmartContractCallRequestDto CallDto { get; }
    }
}
