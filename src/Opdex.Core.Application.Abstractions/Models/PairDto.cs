namespace Opdex.Core.Application.Abstractions.Models
{
    public class PairDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string TokenId { get; set; }
        public decimal CrsLiquidity { get; set; }
        public decimal TokenLiquidity { get; set; }
    }
}