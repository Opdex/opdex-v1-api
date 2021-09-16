using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances
{
    public class CreateAddressBalanceCommand : IRequest<long>
    {
        public CreateAddressBalanceCommand(Address walletAddress, Address token, ulong blockHeight)
        {
            if (walletAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be provided.");
            }

            if (token == Address.Empty)
            {
                throw new ArgumentNullException(nameof(token), "Token address must be provided.");
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than 0.");
            }

            Wallet = walletAddress;
            Token = token;
            Block = blockHeight;
        }

        public Address Wallet { get; }
        public Address Token { get; }
        public ulong Block { get; }
    }
}
