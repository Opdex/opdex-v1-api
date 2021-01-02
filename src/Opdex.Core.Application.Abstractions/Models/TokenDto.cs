namespace Opdex.Core.Application.Abstractions.Models
{
    public class TokenDto
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        // WETH tokens may push this to ulong
        public int Sats { get; set; }
    }
}