using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class Token : BlockAudit
    {
        public Token(Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply, ulong createdBlock)
            : base(createdBlock)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Token address must be set.");
            }

            if (!name.HasValue())
            {
                throw new ArgumentNullException(nameof(name), "Token name must be set.");
            }

            if (!symbol.HasValue())
            {
                throw new ArgumentNullException(nameof(symbol), "Token symbol must be set.");
            }

            if (decimals < 0 || decimals > 18)
            {
                throw new ArgumentOutOfRangeException(nameof(decimals), "Token must have between 0 and 18 decimal denominations.");
            }

            if (sats < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sats), "Sats must be greater than zero.");
            }

            Address = address;
            IsLpt = isLpt;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        public Token(ulong id, Address address, bool isLpt, string name, string symbol, int decimals, ulong sats, UInt256 totalSupply,
                     ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            IsLpt = isLpt;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        public ulong Id { get; }
        public Address Address { get; }
        public bool IsLpt { get; }
        public string Name { get; }
        public string Symbol { get; }
        public int Decimals { get; }
        public ulong Sats { get; }
        public UInt256 TotalSupply { get; private set; }

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
