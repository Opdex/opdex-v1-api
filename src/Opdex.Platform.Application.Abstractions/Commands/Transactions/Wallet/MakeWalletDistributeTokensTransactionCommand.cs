using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletDistributeTokensTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletDistributeTokensTransactionCommand(string walletName, string walletAddress, string walletPassword, string token)
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