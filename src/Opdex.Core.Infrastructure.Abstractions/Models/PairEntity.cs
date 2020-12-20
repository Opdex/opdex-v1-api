namespace Opdex.Core.Infrastructure.Abstractions.Models
{
    public class PairEntity
    {
        public string Token { get; set; }
        public ulong CrsLiquidity { get; set; }
        public ulong TokenLiquidity { get; set; }
    }
}