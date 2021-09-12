using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public abstract class CreateWalletTransactionCommand : IRequest<string>
    {
        protected CreateWalletTransactionCommand(Address walletAddress)
        {
            if (walletAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(walletAddress));
            }

            WalletAddress = walletAddress;
        }

        public Address WalletAddress { get; }
    }
}
