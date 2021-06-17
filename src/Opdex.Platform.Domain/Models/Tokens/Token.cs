using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class Token : BlockAudit
    {
        public Token(string address, bool isLpt, string name, string symbol, int decimals, ulong sats, string totalSupply, ulong createdBlock)
            : base(createdBlock)
        {
            if (!address.HasValue())
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

            if (!totalSupply.IsNumeric())
            {
                throw new ArgumentOutOfRangeException(nameof(totalSupply), "Total supply must only contain numeric digits.");
            }

            Address = address;
            IsLpt = isLpt;
            Name = name;
            Symbol = symbol;
            Decimals = decimals;
            Sats = sats;
            TotalSupply = totalSupply;
        }

        public Token(long id, string address, bool isLpt, string name, string symbol, int decimals, ulong sats, string totalSupply, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
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

        public long Id { get; }
        public string Address { get; }
        public bool IsLpt { get; }
        public string Name { get; }
        public string Symbol { get; }
        public int Decimals { get; }
        public ulong Sats { get; }
        public string TotalSupply { get; private set; }

        public void UpdateTotalSupply(string value, ulong blockHeight)
        {
            if (!value.IsNumeric())
            {
                throw new ArgumentOutOfRangeException("Total supply must be a numeric value");
            }

            TotalSupply = value;
            SetModifiedBlock(blockHeight);
        }
    }
}