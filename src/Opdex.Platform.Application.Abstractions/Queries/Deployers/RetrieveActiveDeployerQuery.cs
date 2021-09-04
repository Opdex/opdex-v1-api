using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Deployers
{
    public class RetrieveActiveDeployerQuery : IRequest<Deployer>
    {
    }
}
