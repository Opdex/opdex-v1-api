using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    /// <summary>
    /// Get the balance of a wallet address and specific token.
    /// </summary>
    public class GetAddressBalanceByTokenQuery : IRequest<AddressBalanceDto>
    {
        /// <summary>
        /// Constructor for get balance query.
        /// </summary>
        /// <param name="walletAddress">The wallet address to check the balance of.</param>
        /// <param name="tokenAddress">The token to check the wallet balance of.</param>
        /// <exception cref="ArgumentNullException">Argument null exception for invalid request parameters.</exception>
        public GetAddressBalanceByTokenQuery(string walletAddress, string tokenAddress)
        {
            WalletAddress = walletAddress.HasValue() ? walletAddress : throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be set.");
            TokenAddress = tokenAddress.HasValue() ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
        }

        public string WalletAddress { get; }
        public string TokenAddress { get; }
    }
}
