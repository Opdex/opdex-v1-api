using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRewardMiningPoolsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletRewardMiningPoolsTransactionCommand(string walletName, string walletAddress, string walletPassword, string governance)
            : base(walletName, walletAddress, walletPassword)
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