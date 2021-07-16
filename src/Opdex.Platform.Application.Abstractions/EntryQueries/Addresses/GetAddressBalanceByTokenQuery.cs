using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressBalanceByTokenQuery : IRequest<AddressBalanceDto>
    {
        public GetAddressBalanceByTokenQuery(string address, string tokenAddress)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
            TokenAddress = address.HasValue() ? tokenAddress : throw new ArgumentNullException(nameof(tokenAddress), "Token address must be set.");
        }

        public string Address { get; }
        public string TokenAddress { get; }
    }
}
