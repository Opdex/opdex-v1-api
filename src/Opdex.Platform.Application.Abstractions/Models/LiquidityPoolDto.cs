namespace Opdex.Platform.Application.Abstractions.Models
{
    public class LiquidityPoolDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public TokenDto Token { get; set; }
    }
}