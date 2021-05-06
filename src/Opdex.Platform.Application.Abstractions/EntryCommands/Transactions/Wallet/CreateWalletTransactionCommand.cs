using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public abstract class CreateWalletTransactionCommand : IRequest<string>
    {
        protected CreateWalletTransactionCommand(string walletName, string walletAddress, string walletPassword)
        {
            if (!walletName.HasValue())
            {
                throw new ArgumentNullException(nameof(walletName));
            }
            
            if (!walletAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(walletAddress));
            }
            
            if (!walletPassword.HasValue())
            {
                throw new ArgumentNullException(nameof(walletPassword));
            }

            WalletName = walletName;
            WalletPassword = walletPassword;
            WalletAddress = walletAddress;
        }
        
        public string WalletName { get; }
        public string WalletAddress { get; }
        public string WalletPassword { get; }
    }
}