using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers
{
    public class SelectDeployerByAddressQuery : FindQuery<Domain.Models.Deployer>
    {
        public SelectDeployerByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }
        
        public string Address { get; }
    }
}