using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Admins;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Admins
{
    public class SelectAdminByAddressQuery : FindQuery<Admin>
    {
        public SelectAdminByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            Address = address != Address.Empty ? address : throw new ArgumentException("Address must not be empty.", nameof(address));
        }

        public Address Address { get; }
    }
}
