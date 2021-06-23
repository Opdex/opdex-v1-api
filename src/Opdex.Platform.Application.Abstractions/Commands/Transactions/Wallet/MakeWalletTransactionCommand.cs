using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public abstract class MakeWalletTransactionCommand : IRequest<string>
    {
        protected MakeWalletTransactionCommand(string walletAddress)
        {
            if (!walletAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(walletAddress));
            }

            WalletName = "cirrusdev";
            WalletPassword = "password";
            WalletAddress = walletAddress;
        }

        public string WalletName { get; }
        public string WalletAddress { get; }
        public string WalletPassword { get; }
    }
}
