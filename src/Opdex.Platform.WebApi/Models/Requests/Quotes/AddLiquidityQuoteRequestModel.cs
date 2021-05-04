namespace Opdex.Platform.WebApi.Models.Requests.Quotes
{
    public class AddLiquidityQuoteRequestModel
    {
        public string AmountCrsIn { get; set; }
        
        public string AmountSrcIn { get; set; }

        public string Pool { get; set; }
        
        public string Market { get; set; }
    }
}