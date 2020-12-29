namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class SwapEventEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string FromToken { get; set; }
        public string ToToken { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string TxHash { get; set; }
    }
}