using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressBalanceByTokenQuery : IRequest<AddressBalanceDto>
    {
        public GetAddressBalanceByTokenQuery(string walletAddress, string tokenAddress)
        {
            WalletAddress = walletAddress.HasValue() ? walletAddress : throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be set.");
            TokenAddress = tokenAddress.HasValue() ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
        }

        public string WalletAddress { get; }
        public string TokenAddress { get; }
    }
}
