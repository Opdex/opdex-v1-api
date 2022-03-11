using Nethereum.Util;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class TokenWrapped : BlockAudit
{
    public TokenWrapped(ulong tokenId, Address owner, ExternalChainType nativeChain, string nativeAddress, bool trusted, ulong createdBlock)
        : base(createdBlock)
    {
        if (tokenId == 0) throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than 0.");
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        if (!nativeChain.IsValid()) throw new ArgumentOutOfRangeException(nameof(nativeChain), "Chain type must be valid.");
        TokenId = tokenId;
        Owner = owner;
        Validated = TryValidateWrapping(nativeChain, nativeAddress, out var formattedAddress);
        NativeChain = nativeChain;
        Trusted = trusted;
        NativeAddress = formattedAddress;
    }

    public TokenWrapped(ulong id, ulong tokenId, Address owner, ExternalChainType nativeChain, string nativeAddress,
        bool validated, bool trusted, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        TokenId = tokenId;
        Owner = owner;
        NativeChain = nativeChain;
        NativeAddress = nativeAddress;
        Validated = validated;
        Trusted = trusted;
    }

    public ulong Id { get; }
    public ulong TokenId { get; }
    public Address Owner { get; private set; }
    public ExternalChainType NativeChain { get; }
    public string NativeAddress { get; }
    public bool Validated { get; }
    public bool Trusted { get; }

    public void SetOwner(Address owner, ulong blockHeight)
    {
        if (owner == Address.Empty) throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        Owner = owner;
        SetModifiedBlock(blockHeight);
    }

    private static bool TryValidateWrapping(ExternalChainType nativeChain, string nativeAddress, out string formattedAddress)
    {
        formattedAddress = nativeAddress;

        // base token of chain should have null native address
        if (nativeAddress is null) return true;

        switch (nativeChain)
        {
            case ExternalChainType.Ethereum:
                if (!nativeAddress.StartsWith("0x")) nativeAddress = nativeAddress.Insert(0, "0x");
                var isValid = !AddressUtil.Current.IsAnEmptyAddress(nativeAddress)
                              && AddressUtil.Current.IsValidEthereumAddressHexFormat(nativeAddress);
                if (isValid) formattedAddress = AddressUtil.Current.ConvertToChecksumAddress(nativeAddress);
                return isValid;
            default: return false;
        }
    }
}
