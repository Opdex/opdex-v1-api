using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletDistributeTokensTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletDistributeTokensTransactionCommand(string walletName, string walletAddress, string walletPassword, string token)
            : base(walletName, walletAddress, walletPassword)
        {
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
        }
        
        public string Token { get; }
    }
}