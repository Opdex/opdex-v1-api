namespace Opdex.BasePlatform.Infrastructure.Abstractions.Models
{
    public class SwapEventEntity
    {
        public string From { get; set; }
        public string To { get; set; }
        public string FromToken { get; set; }
        public string ToToken { get; set; }
        public ulong FromAmount { get; set; }
        public ulong ToAmount { get; set; }
        public string TxHash { get; set; }
    }
}