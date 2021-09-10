using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SyncRequest
    {
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }
    }
}
