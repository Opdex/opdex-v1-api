using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class TokenChain
{
    public TokenChain(ulong tokenId, ExternalChainType nativeChain, string nativeAddress)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        if (!nativeChain.IsValid()) throw new ArgumentOutOfRangeException(nameof(nativeChain), "Chain type must be valid.");
        if (!nativeAddress.HasValue()) throw new ArgumentNullException(nameof(nativeAddress), "Native address must be set.");
        TokenId = tokenId;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
    }

    public TokenChain(ulong id, ulong tokenId, ExternalChainType nativeChain, string nativeAddress)
    {
        Id = id;
        TokenId = tokenId;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
    }

    public ulong Id { get; }
    public ulong TokenId { get; }
    public ExternalChainType NativeChain { get; }
    public string NativeAddress { get; }
}
