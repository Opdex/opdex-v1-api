using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletStopMiningTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletStopMiningTransactionCommand(Address walletAddress, Address miningPool, UInt256 amount) : base(walletAddress)
        {
            if (miningPool == Address.Empty)
            {
                throw new ArgumentNullException(nameof(miningPool));
            }

            MiningPool = miningPool;
            Amount = amount;
        }

        public Address MiningPool { get; }
        public UInt256 Amount { get; }
    }
}
