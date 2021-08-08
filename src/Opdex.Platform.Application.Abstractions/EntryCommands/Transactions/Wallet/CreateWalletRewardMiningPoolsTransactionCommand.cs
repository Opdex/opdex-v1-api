using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRewardMiningPoolsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRewardMiningPoolsTransactionCommand(string walletAddress, string governance) : base(walletAddress)
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
