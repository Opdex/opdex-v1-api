using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults
{
    public class GetAllVaultsQuery : IRequest<IEnumerable<VaultDto>>
    {
    }
}
