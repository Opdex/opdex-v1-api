using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    /// <summary>
    /// Get the balance of a wallet address and specific token.
    /// </summary>
    public class GetAddressBalanceByTokenQuery : IRequest<AddressBalanceDto>
    {
        /// <summary>
        /// Creates a request to retrieve a balance query.
        /// </summary>
        /// <param name="walletAddress">The wallet address to check the balance of.</param>
        /// <param name="tokenAddress">The token to check the wallet balance of.</param>
        /// <exception cref="ArgumentNullException">Argument null exception for invalid request parameters.</exception>
        public GetAddressBalanceByTokenQuery(Address walletAddress, Address tokenAddress)
        {
            WalletAddress = walletAddress != Address.Empty ? walletAddress : throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be set.");
            TokenAddress = tokenAddress != Address.Empty ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
        }

        public Address WalletAddress { get; }
        public Address TokenAddress { get; }
    }
}
