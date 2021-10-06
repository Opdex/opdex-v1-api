using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class Token : BaseToken
    {
        public Token(Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply, ulong createdBlock)
            : base(address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock)
        {
        }

        public Token(ulong id, Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply,
                     ulong createdBlock, ulong modifiedBlock)
            : base(address, isLpt, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock)
        {
            Id = id;
        }

        public ulong Id { get; }

        // Todo: Look into and fix or document fix with future task for market_tokens
        // Being used to tie a token to a market so the Assembler can fetch pricing
        // Todo: Remove after assemblers are adjusted for Market vs Global tokens
        public ulong? MarketId { get; private set; }

        public void UpdateTotalSupply(UInt256 value, ulong blockHeight)
        {
            TotalSupply = value;
            SetModifiedBlock(blockHeight);
        }

        public void SetMarket(ulong marketId)
        {
            MarketId = marketId;
        }

        public void Update(StandardTokenContractSummary summary)
        {
            if (summary.TotalSupply.HasValue) TotalSupply = summary.TotalSupply.Value;
            SetModifiedBlock(summary.BlockHeight);
        }
    }
}
