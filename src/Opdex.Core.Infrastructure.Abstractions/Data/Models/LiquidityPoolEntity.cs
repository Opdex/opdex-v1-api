namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class LiquidityPoolEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long TokenId { get; set; }
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}