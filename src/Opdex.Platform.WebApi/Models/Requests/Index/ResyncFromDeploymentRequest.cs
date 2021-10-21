using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    public class ResyncFromDeploymentRequest
    {
        public Sha256 MinedTokenDeploymentHash { get; set; }
        public Sha256 MarketDeployerDeploymentTxHash { get; set; }
    }
}
