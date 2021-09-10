using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletSkimTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletSkimTransactionCommand(Address walletAddress, Address liquidityPool, Address recipient) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            if (recipient == Address.Empty)
            {
                throw new ArgumentNullException(nameof(recipient));
            }

            LiquidityPool = liquidityPool;
            Recipient = recipient;
        }

        public Address LiquidityPool { get; }
        public Address Recipient { get; }
    }
}
