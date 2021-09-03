namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    public class ResyncFromDeploymentRequest
    {
        public string MinedTokenDeploymentHash { get; set; }
        public string MarketDeployerDeploymentTxHash { get; set; }
    }
}
