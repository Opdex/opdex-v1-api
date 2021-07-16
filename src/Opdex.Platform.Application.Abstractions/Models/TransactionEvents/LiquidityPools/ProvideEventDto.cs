using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools
{
    public class ProvideEventDto : TransactionEventDto
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public string AmountCrs { get; set; }
        public string AmountSrc { get; set; }
        public string AmountLpt { get; set; }
        public string TokenSrc { get; set; }
        public string TokenLp { get; set; }
    }
}
