namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    public class ResyncFromDeploymentRequest
    {
        public string OdxDeploymentTxHash { get; set; }
        public string MarketDeployerDeploymentTxHash { get; set; }
    }
}