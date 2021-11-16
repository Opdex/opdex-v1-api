using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class RewardsResponseModel
    {
        [Range(0, double.MaxValue)]
        public decimal ProviderUsd { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MarketUsd { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal TotalUsd { get; set; }
    }
}