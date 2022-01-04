using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.Tokens;

public class Token : TokenBase
{
    public Token(Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply, ulong createdBlock)
        : base(address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock)
    {
    }

    public Token(ulong id, Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply,
                 ulong createdBlock, ulong modifiedBlock)
        : base(id, address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock)
    {
    }

    public void UpdateTotalSupply(UInt256 value, ulong blockHeight)
    {
        TotalSupply = value;
        SetModifiedBlock(blockHeight);
    }

    public void Update(StandardTokenContractSummary summary)
    {
        if (summary.TotalSupply.HasValue) TotalSupply = summary.TotalSupply.Value;
        SetModifiedBlock(summary.BlockHeight);
    }
}
