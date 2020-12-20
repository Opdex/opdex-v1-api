namespace Opdex.Core.Infrastructure.Abstractions.Models
{
    public class TokenEntity
    {
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Decimals { get; set; }
        public int Sats { get; set; }
        public ulong MaxSupply { get; set; }
    }
}