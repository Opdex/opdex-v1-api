using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class TokenWrapped : BlockAudit
{
    public TokenWrapped(ulong tokenId, Address owner, ExternalChainType nativeChain, string nativeAddress, ulong createdBlock)
        : base(createdBlock)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        if (!nativeChain.IsValid()) throw new ArgumentOutOfRangeException(nameof(nativeChain), "Chain type must be valid.");
        TokenId = tokenId;
        Owner = owner;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
    }

    public TokenWrapped(ulong id, ulong tokenId, Address owner, ExternalChainType nativeChain, string nativeAddress,
        ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        TokenId = tokenId;
        Owner = owner;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
    }

    public ulong Id { get; }
    public ulong TokenId { get; }
    public Address Owner { get; private set; }
    public ExternalChainType NativeChain { get; }
    public string NativeAddress { get; }

    public void SetOwner(Address owner, ulong blockHeight)
    {
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        Owner = owner;
        SetModifiedBlock(blockHeight);
    }
}
