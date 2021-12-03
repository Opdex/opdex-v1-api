using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Mining;

/// <summary>
/// Retrieves a mining position of a particular address in a mining pool
/// /// </summary>
public class GetMiningPositionByPoolQuery : IRequest<MiningPositionDto>
{
    public GetMiningPositionByPoolQuery(Address miner, Address miningPool)
    {
        Address = miner != Address.Empty ? miner : throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
        MiningPoolAddress = miningPool != Address.Empty ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
    }

    public Address Address { get; }
    public Address MiningPoolAddress { get; }
}