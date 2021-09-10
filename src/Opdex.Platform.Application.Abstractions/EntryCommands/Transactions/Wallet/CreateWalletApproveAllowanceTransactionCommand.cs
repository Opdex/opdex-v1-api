using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletApproveAllowanceTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletApproveAllowanceTransactionCommand(Address walletAddress, Address token, FixedDecimal amount, Address spender) : base(walletAddress)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (spender == Address.Empty)
            {
                throw new ArgumentNullException(nameof(spender));
            }

            Token = token;
            Amount = amount;
            Spender = spender;
        }

        public Address Token { get; }
        public FixedDecimal Amount { get; }
        public Address Spender { get; }
    }
}
