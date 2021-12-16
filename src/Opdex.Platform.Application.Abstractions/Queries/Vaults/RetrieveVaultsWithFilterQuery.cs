using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

public class RetrieveVaultsWithFilterQuery : IRequest<IEnumerable<Vault>>
{
    public RetrieveVaultsWithFilterQuery(VaultsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public VaultsCursor Cursor { get; }
}
