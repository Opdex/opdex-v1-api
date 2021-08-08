using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectAllVaultsQuery : IRequest<IEnumerable<Vault>>
    {
    }
}
