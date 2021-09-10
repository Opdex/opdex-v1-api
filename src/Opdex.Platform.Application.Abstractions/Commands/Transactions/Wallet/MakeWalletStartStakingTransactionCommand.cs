using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStartStakingTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStartStakingTransactionCommand(Address walletAddress, UInt256 amount, Address liquidityPool) : base(walletAddress)
        {
            if (liquidityPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(liquidityPool));
            }

            Amount = amount;
            LiquidityPool = liquidityPool;
        }

        public UInt256 Amount { get; }
        public Address LiquidityPool { get; }
    }
}
