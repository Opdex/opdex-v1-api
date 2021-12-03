using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;

/// <summary>Query to retrieve the mining balance of an address.</summary>
public class CallCirrusGetMiningBalanceForAddressQuery : IRequest<UInt256>
{
    /// <summary>Creates a query to retrieve the mining balance of an address. </summary>
    /// <param name="miningPool">The address of the mining pool.</param>
    /// <param name="miner">The address of the miner.</param>
    /// <param name="blockHeight">Block height to query at.</param>
    public CallCirrusGetMiningBalanceForAddressQuery(Address miningPool, Address miner, ulong blockHeight)
    {
        if (miningPool == Address.Empty) throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
        if (miner == Address.Empty) throw new ArgumentNullException(nameof(miner), "Miner address must be set.");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");

        MiningPool = miningPool;
        Miner = miner;
        BlockHeight = blockHeight;
    }

    public Address MiningPool { get; }
    public Address Miner { get; }
    public ulong BlockHeight { get; }
}