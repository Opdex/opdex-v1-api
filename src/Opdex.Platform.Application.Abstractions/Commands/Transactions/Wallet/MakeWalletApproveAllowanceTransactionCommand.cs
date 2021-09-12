using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletApproveAllowanceTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletApproveAllowanceTransactionCommand(Address walletAddress, Address token, UInt256 amount, Address spender) : base(walletAddress)
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
        public UInt256 Amount { get; }
        public Address Spender { get; }
    }
}
