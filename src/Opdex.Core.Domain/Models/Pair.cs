namespace Opdex.Core.Domain.Models
{
    public class Pair
    {
        public Pair (long id, string address, string tokenId, decimal crsLiquidity, decimal tokenLiquidity)
        {
            // Todo: Add validations and actually final thought properties
            Id = id;
            Address = address;
            TokenId = tokenId;
            CrsLiquidity = crsLiquidity;
            TokenLiquidity = tokenLiquidity;
        }
        public long Id { get; }
        public string Address { get; }
        public string TokenId { get; }
        public decimal CrsLiquidity { get; }
        public decimal TokenLiquidity { get; }
    }
}