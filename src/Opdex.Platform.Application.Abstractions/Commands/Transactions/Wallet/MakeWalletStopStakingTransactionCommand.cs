using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStopStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStopStakingTransactionCommand(Address walletAddress, Address liquidityPool, UInt256 amount, bool liquidate) : base(walletAddress)
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
        public UInt256 Amount { get; }
    }
}
