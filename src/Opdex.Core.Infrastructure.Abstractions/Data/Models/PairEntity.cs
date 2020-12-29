namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class PairEntity
    {
        public long Id { get; set; }
        public string Token { get; set; }
        public decimal CrsLiquidity { get; set; }
        public decimal TokenLiquidity { get; set; }
    }
}