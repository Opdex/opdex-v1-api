using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public abstract class CreateWalletTransactionCommand : IRequest<string>
    {
        protected CreateWalletTransactionCommand(string walletAddress)
        {
            if (!walletAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(walletAddress));
            }

            WalletAddress = walletAddress;
        }

        public string WalletAddress { get; }
    }
}
