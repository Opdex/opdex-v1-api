using Opdex.Platform.Application.Abstractions.Models.Admins;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Admins;

public class GetAdminByAddressQuery : FindQuery<AdminDto>
{
    public GetAdminByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must not be empty.");
    }

    public Address Address { get; }
}