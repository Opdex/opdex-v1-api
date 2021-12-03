using MediatR;
using Opdex.Platform.Domain.Models.Deployers;

namespace Opdex.Platform.Application.Abstractions.Queries.Deployers;

public class RetrieveActiveDeployerQuery : IRequest<Deployer>
{
}