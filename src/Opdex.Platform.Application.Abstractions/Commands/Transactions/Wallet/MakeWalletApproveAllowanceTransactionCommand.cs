using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletApproveAllowanceTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletApproveAllowanceTransactionCommand(string walletName, string walletAddress, string walletPassword, 
            string token, string amount, string spender) : base(walletName, walletAddress, walletPassword)
        {
            if (!token.HasValue())
            {
                throw new ArgumentNullException(nameof(token));
            }
            
            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
            }
            
            if (!spender.HasValue())
            {
                throw new ArgumentNullException(nameof(spender));
            }
            
            Token = token;
            Amount = amount;
            Spender = spender;
        }
        
        public string Token { get; }
        public string Amount { get; }
        public string Spender { get; }
    }
}