using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRewardMiningPoolsTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletRewardMiningPoolsTransactionCommand(Address walletAddress, Address governance) : base(walletAddress)
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
