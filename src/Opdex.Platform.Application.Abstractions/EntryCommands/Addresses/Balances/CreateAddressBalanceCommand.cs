using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances
{
    /// <summary>
    /// Create or updates an address balance record based on the modified block of the record and the provided block height.
    /// </summary>
    public class CreateAddressBalanceCommand : IRequest<long>
    {
        /// <summary>
        /// Create the create address balance command.
        /// </summary>
        /// <param name="walletAddress">The wallet address that holds the balance.</param>
        /// <param name="token">The address of the token the balance represents.</param>
        /// <param name="blockHeight">The block height of the balance modification.</param>
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
