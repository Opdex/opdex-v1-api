namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSummaryResponseModel
    {
        public decimal PriceUsd { get; set; }
        public decimal DailyPriceChangePercent { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}
