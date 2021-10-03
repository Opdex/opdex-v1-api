using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults
{
    public class SelectVaultByTokenIdQuery : FindQuery<Vault>
    {
        public SelectVaultByTokenIdQuery(ulong tokenId, bool findOrThrow) : base(findOrThrow)
        {
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
        }

        public ulong TokenId { get; }
    }
}
