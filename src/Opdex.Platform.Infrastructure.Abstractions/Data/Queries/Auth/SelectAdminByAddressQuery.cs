using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Auth;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;

public class SelectAdminByAddressQuery : FindQuery<Admin>
{
    public SelectAdminByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must not be empty.");
    }

    public Address Address { get; }
}
