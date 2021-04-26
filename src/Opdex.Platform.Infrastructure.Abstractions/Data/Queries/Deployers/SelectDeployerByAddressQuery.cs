using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers
{
    public class SelectDeployerByAddressQuery : IRequest<Domain.Models.Deployer>
    {
        public SelectDeployerByAddressQuery(string address)
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