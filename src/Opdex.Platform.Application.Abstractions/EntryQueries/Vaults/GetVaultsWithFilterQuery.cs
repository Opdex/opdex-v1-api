using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;

public class GetVaultsWithFilterQuery : IRequest<VaultsDto>
{
    public GetVaultsWithFilterQuery(VaultsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public VaultsCursor Cursor { get; }
}