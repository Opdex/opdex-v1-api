using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    public class SwapEventResponseModel : TransactionEventResponseModel
    {
        public Address Sender { get; set; }
        public Address To { get; set; }
        public FixedDecimal AmountCrsIn { get; set; }
        public FixedDecimal AmountSrcIn { get; set; }
        public FixedDecimal AmountCrsOut { get; set; }
        public FixedDecimal AmountSrcOut { get; set; }
        public Address SrcToken { get; set; }
    }
}
