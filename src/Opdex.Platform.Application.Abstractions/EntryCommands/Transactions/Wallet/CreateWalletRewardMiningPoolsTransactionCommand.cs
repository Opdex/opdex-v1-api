using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRewardMiningPoolsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRewardMiningPoolsTransactionCommand(string walletName, string walletAddress, string walletPassword, string governance)
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