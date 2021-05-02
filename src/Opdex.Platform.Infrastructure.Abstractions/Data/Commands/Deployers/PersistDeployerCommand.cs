using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers
{
    public class PersistDeployerCommand : IRequest<long>
    {
        public PersistDeployerCommand(string address)
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