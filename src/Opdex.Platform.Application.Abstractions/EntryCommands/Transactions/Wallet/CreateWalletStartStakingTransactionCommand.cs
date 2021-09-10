using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletStartStakingTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletStartStakingTransactionCommand(Address walletAddress, FixedDecimal amount, Address liquidityPool) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            Amount = amount;
            LiquidityPool = liquidityPool;
        }

        public FixedDecimal Amount { get; }
        public Address LiquidityPool { get; }
    }
}
