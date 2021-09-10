using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCreateLiquidityPoolTransactionCommand : CreateWalletTransactionCommand
    {
        public CreateWalletCreateLiquidityPoolTransactionCommand(Address walletAddress, Address token, Address market) : base(walletAddress)
        {
            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (market == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token));
            }

            Token = token;
            Market = market;
        }

        public Address Token { get; }
        public Address Market { get; }
    }
}
