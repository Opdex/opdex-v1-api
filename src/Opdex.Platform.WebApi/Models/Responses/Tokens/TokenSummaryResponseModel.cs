using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSummaryResponseModel
    {
        [Range(0, double.MaxValue)]
        public decimal PriceUsd { get; set; }

        public decimal DailyPriceChangePercent { get; set; }

        [Range(1, double.MaxValue)]
        public ulong ModifiedBlock { get; set; }
    }
}
