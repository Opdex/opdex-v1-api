using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Deployers
{
    public class MakeDeployerCommand : IRequest<long>
    {
        public MakeDeployerCommand(Deployer deployer)
        {
            Deployer = deployer ?? throw new ArgumentNullException(nameof(deployer));
        }
        
        public Deployer Deployer { get; }
    }
}