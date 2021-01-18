namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class PairEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string TokenId { get; set; }
        public decimal ReserveToken { get; set; }
        public decimal ReserveCrs { get; set; }
    }
}