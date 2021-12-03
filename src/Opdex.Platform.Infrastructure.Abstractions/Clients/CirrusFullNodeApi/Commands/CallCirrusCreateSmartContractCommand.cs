using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;

/// <summary>
/// Generic command to create a smart contract and broadcast the transaction. Intended only for contract deployment.
/// </summary>
public class CallCirrusCreateSmartContractCommand : IRequest<Sha256>
{
    /// <summary>
    /// Creates a command to create a smart contract and broadcast the transaction.
    /// </summary>
    /// <param name="requestDto">The parameters of the create smart contract request.</param>
    public CallCirrusCreateSmartContractCommand(SmartContractCreateRequestDto requestDto)
    {
        RequestDto = requestDto ?? throw new ArgumentNullException(nameof(requestDto));
    }

    public SmartContractCreateRequestDto RequestDto { get; }
}