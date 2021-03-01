namespace Opdex.Platform.WebApi.Models.Requests.Quotes
{
    public class SwapQuoteRequestModel
    {
        public string TokenIn { get; set; }
        public string TokenOut { get; set; }
        
        public string TokenInAmount { get; set; }
        public string TokenOutAmount { get; set; }
        
        public decimal Tolerance { get; set; }
        
        public ulong Deadline { get; set; }
    }
}