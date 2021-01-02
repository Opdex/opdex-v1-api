using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Token
    {
        public Token(string address, string name, string symbol, int decimals, int sats)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
            Name = name.HasValue() ? name : throw new ArgumentNullException(nameof(name));
            Symbol = symbol.HasValue() ? symbol : throw new ArgumentNullException(nameof(symbol));
            Decimals = decimals >= 0 ? decimals : throw new ArgumentOutOfRangeException(nameof(decimals));
            Sats = sats >= 0 ? sats : throw new ArgumentOutOfRangeException(nameof(sats));
        }
        
        public string Address { get; }
        public string Name { get; }
        public string Symbol { get; }
        public int Decimals { get; }
        // WETH tokens may push this to ulong
        public int Sats { get; }
    }
}