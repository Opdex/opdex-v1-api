using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class SwapEventDto : TransactionEventDto
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public string AmountCrsIn { get; set; }
        public string AmountSrcIn { get; set; }
        public string AmountCrsOut { get; set; }
        public string AmountSrcOut { get; set; }
        public string SrcToken { get; set; }
    }
}
