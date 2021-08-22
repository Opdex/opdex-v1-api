using System;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SyncRequest
    {
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public string LiquidityPool { get; set; }
    }
}
