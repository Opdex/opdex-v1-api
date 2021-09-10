using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletDistributeTokensTransactionCommand : MakeWalletTransactionCommand
    {
        public MakeWalletDistributeTokensTransactionCommand(Address walletAddress, Address token) : base(walletAddress)
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
