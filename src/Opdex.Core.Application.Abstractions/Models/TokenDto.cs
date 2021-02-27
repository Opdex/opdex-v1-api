namespace Opdex.Core.Application.Abstractions.Models
{
    public class TokenDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public long Sats { get; set; }
        public string TotalSupply { get; set; }
    }
}