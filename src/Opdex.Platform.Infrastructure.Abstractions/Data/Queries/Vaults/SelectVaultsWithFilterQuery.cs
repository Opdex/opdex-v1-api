using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

public class SelectVaultsWithFilterQuery : IRequest<IEnumerable<Vault>>
{
    public SelectVaultsWithFilterQuery(VaultsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public VaultsCursor Cursor { get; }
}
