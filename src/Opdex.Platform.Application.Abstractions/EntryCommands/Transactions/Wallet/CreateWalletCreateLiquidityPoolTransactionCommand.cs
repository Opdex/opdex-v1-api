using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCreateLiquidityPoolTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletCreateLiquidityPoolTransactionCommand(string walletName, string walletAddress, string walletPassword, string token, string market)
            : base(walletName, walletAddress, walletPassword)
        {
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!market.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            Token = token;
            Market = market;
        }
        
        public string Token { get; }
        public string Market { get; }
    }
}