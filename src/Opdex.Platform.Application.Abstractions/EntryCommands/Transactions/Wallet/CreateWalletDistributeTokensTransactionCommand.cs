using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletDistributeTokensTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletDistributeTokensTransactionCommand(Address walletAddress, Address token) : base(walletAddress)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
        }

        public Address Token { get; }
    }
}
