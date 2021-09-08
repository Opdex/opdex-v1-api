using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Admins;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Admins
{
    public class RetrieveAdminByAddressQuery : FindQuery<Admin>
    {
        public RetrieveAdminByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            Address = address != Address.Empty ? address : throw new ArgumentException("Address must not be empty.", nameof(address));
        }

        public Address Address { get; }
    }
}
