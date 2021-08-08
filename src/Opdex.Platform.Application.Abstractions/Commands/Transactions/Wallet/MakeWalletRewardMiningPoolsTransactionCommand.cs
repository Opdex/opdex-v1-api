using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRewardMiningPoolsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletRewardMiningPoolsTransactionCommand(string walletAddress, string governance)
            : base(walletAddress)
        {
            if (!governance.HasValue())
            {
                throw new ArgumentNullException(nameof(governance));
            }

            Governance = governance;
        }

        public string Governance { get; }
    }
}
