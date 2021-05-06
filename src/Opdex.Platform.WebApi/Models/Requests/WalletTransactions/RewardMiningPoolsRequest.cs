namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class RewardMiningPoolsRequest : LocalWalletCredentials
    {
        /// <summary>
        /// The address of the mining governance contract to call.
        /// </summary>
        public string Governance { get; set; }
    }
}