using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults
{
    public class GetVaultByAddressQuery : IRequest<VaultDto>
    {
        public GetVaultByAddressQuery(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        }

        public string Address { get; }
    }
}
