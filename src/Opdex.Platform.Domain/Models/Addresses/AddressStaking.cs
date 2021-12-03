using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses;

public class AddressStaking : BlockAudit
{
    public AddressStaking(ulong liquidityPoolId, Address owner, UInt256 weight, ulong createdBlock) : base(createdBlock)
    {
        if (liquidityPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "Liquidity pool id must be greater than 0.");
        }

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner must be set.");
        }

        LiquidityPoolId = liquidityPoolId;
        Owner = owner;
        Weight = weight;
    }

    public AddressStaking(ulong id, ulong liquidityPoolId, Address owner, UInt256 weight, ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        LiquidityPoolId = liquidityPoolId;
        Owner = owner;
        Weight = weight;
    }

    public ulong Id { get; }
    public ulong LiquidityPoolId { get; }
    public Address Owner { get; }
    public UInt256 Weight { get; private set; }

    public void SetWeight(UInt256 weight, ulong block)
    {
        Weight = weight;
        SetModifiedBlock(block);
    }
}