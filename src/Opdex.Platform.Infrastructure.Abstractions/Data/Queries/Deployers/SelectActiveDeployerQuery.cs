using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers
{
    public class SelectActiveDeployerQuery : IRequest<Deployer>
    {
    }
}
