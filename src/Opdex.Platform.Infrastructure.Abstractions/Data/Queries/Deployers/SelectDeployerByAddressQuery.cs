using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Deployers;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;

public class SelectDeployerByAddressQuery : FindQuery<Deployer>
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