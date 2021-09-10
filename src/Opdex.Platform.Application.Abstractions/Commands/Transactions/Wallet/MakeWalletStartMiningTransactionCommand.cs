using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStartMiningTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStartMiningTransactionCommand(Address walletAddress, UInt256 amount, Address miningPool) : base(walletAddress)
        {
            if (miningPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            Amount = amount;
            MiningPool = miningPool;
        }

        public UInt256 Amount { get; }
        public Address MiningPool { get; }
    }
}
