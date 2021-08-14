using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultsWithFilterQuery : IRequest<IEnumerable<Vault>>
    {
        public SelectVaultsWithFilterQuery(VaultsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public VaultsCursor Cursor { get; }
    }
}
