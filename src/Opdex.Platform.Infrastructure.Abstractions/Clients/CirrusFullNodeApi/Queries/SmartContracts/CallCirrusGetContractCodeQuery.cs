using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;

/// <summary>
/// Call cirrus and get the contract code details.
/// </summary>
public class CallCirrusGetContractCodeQuery : IRequest<ContractCodeDto>
{
    /// <summary>
    /// Create a new Call Cirrus Get Contract Code Query by contract address
    /// </summary>
    /// <param name="contract">The address of the contract to check.</param>
    public CallCirrusGetContractCodeQuery(Address contract)
    {
        Contract = contract != Address.Empty ? contract : throw new ArgumentNullException(nameof(contract), "Contract must be a valid address.");
    }

    public Address Contract { get; }
}
