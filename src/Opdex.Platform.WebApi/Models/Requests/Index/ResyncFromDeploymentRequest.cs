using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Index
{
    /// <summary>
    /// Request to reindex from an Opdex smart contract deployment.
    /// </summary>
    public class ResyncFromDeploymentRequest
    {
        /// <summary>
        /// SHA-256 hash of the governance token contract creation transaction.
        /// </summary>
        /// <example>b44ccc7fc8b1428fa5e0db4d40d8602531840e805d833585030a38489bd816ee</example>
        public Sha256 MinedTokenDeploymentHash { get; set; }

        /// <summary>
        /// SHA-256 hash of the market deployer contract creation transaction.
        /// </summary>
        /// <example>27f82a88fa12c84081bf5d0ac091da854d962edfce26586bf0aa7c5b36564d7e</example>
        public Sha256 MarketDeployerDeploymentTxHash { get; set; }
    }
}
