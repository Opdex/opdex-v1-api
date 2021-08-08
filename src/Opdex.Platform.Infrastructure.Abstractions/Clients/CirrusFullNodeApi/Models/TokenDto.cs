namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class TokenDto
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Ticker { get; set; }
        public short Decimals { get; set; }
    }
}