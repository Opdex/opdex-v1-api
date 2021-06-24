namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class MiningPoolResponseModel
    {
        public string Address { get; set; }
        public string IsActive { get; set; }
        public string RewardPerBlock { get; set; }
        public string MiningLpt { get; set; }
        public string MiningUsd { get; set; }
        public string MiningPeriodEnd { get; set; }
    }
}
