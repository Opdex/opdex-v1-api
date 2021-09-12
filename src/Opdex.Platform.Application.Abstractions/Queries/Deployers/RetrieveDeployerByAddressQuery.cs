using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Deployers
{
    public class RetrieveDeployerByAddressQuery : FindQuery<Deployer>
    {
        public RetrieveDeployerByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
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
