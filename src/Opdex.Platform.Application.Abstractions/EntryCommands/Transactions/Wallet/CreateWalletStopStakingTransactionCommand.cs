using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStopStakingTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStopStakingTransactionCommand(Address walletAddress, Address liquidityPool, FixedDecimal amount, bool liquidate) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            LiquidityPool = liquidityPool;
            Liquidate = liquidate;
            Amount = amount;
        }

        public Address LiquidityPool { get; }
        public bool Liquidate { get; }
        public FixedDecimal Amount { get; }
    }
}
