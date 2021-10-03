using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultByTokenIdQuery : FindQuery<Vault>
    {
        public RetrieveVaultByTokenIdQuery(ulong tokenId, bool findOrThrow) : base(findOrThrow)
        {
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "Token id must be greater than zero.");
        }

        public ulong TokenId { get; }
    }
}
