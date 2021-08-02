namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public abstract class ProvideEventDto : TransactionEventDto
    {
        public string AmountCrs { get; set; }
        public string AmountSrc { get; set; }
        public string AmountLpt { get; set; }
        public string TokenSrc { get; set; }
        public string TokenLp { get; set; }
        public string TokenLpTotalSupply { get; set; }
    }
}
