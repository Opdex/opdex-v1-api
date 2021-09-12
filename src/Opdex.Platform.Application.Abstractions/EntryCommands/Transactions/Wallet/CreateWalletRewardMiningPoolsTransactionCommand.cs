using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRewardMiningPoolsTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletRewardMiningPoolsTransactionCommand(Address walletAddress, Address governance) : base(walletAddress)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance));
            }

            Governance = governance;
        }

        public Address Governance { get; }
    }
}
