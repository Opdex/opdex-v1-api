using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveAllVaultsQuery : IRequest<IEnumerable<Vault>>
    {
    }
}
