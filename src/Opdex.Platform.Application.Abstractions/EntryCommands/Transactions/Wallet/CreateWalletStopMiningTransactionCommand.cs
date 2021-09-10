using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStopMiningTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStopMiningTransactionCommand(Address walletAddress, Address liquidityPool, FixedDecimal amount) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Amount = amount;
        }

        public Address LiquidityPool { get; }
        public FixedDecimal Amount { get; }
    }
}
