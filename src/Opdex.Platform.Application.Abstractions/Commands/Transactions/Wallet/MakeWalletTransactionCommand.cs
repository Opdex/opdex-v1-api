using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public abstract class MakeWalletTransactionCommand : IRequest<string>
    {
        protected MakeWalletTransactionCommand(Address walletAddress)
        {
            if (walletAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(walletAddress));
            }

            WalletName = "cirrusdev";
            WalletPassword = "password";
            WalletAddress = walletAddress;
        }

        public string WalletName { get; }
        public Address WalletAddress { get; }
        public string WalletPassword { get; }
    }
}
