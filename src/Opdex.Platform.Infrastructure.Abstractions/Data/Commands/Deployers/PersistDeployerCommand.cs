using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers
{
    public class PersistDeployerCommand : IRequest<long>
    {
        public PersistDeployerCommand(Deployer deployer)
        {
            Deployer = deployer ?? throw new ArgumentNullException(nameof(deployer));
        }
        
        public Deployer Deployer { get; }
    }
}