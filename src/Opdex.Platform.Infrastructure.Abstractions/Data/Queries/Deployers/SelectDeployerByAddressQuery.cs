using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers
{
    public class SelectDeployerByAddressQuery : FindQuery<Domain.Models.Deployer>
    {
        public SelectDeployerByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
