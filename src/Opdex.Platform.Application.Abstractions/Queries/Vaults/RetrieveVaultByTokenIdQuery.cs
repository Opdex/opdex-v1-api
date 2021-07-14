using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.ODX;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultByTokenIdQuery : FindQuery<Vault>
    {
        public RetrieveVaultByTokenIdQuery(long tokenId, bool findOrThrow) : base(findOrThrow)
        {
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
        }

        public long TokenId { get; }
    }
}