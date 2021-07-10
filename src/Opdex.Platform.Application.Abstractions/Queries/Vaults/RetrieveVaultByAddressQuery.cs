using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultByAddressQuery : FindQuery<Vault>
    {
        public RetrieveVaultByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        }

        public string Address { get; }
    }
}
