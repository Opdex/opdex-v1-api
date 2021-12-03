using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Addresses;

public class AddressBalance : BlockAudit
{
    public AddressBalance(ulong tokenId, Address owner, UInt256 balance, ulong createdBlock) : base(createdBlock)
    {
        if (tokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        }

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner must be set.");
        }

        TokenId = tokenId;
        Owner = owner;
        Balance = balance;
    }

    public AddressBalance(ulong id, ulong tokenId, Address owner, UInt256 balance, ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        TokenId = tokenId;
        Owner = owner;
        Balance = balance;
    }

    public ulong Id { get; }
    public ulong TokenId { get; }
    public Address Owner { get; }
    public UInt256 Balance { get; private set; }

    public void SetBalance(UInt256 balance, ulong blockHeight)
    {
        Balance = balance;
        SetModifiedBlock(blockHeight);
    }
}