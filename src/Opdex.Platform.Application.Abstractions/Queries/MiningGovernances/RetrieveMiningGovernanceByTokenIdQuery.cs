using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;

public class RetrieveMiningGovernanceByTokenIdQuery : FindQuery<MiningGovernance>
{
    public RetrieveMiningGovernanceByTokenIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
    {
        if (tokenId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0");
        }

        TokenId = tokenId;
    }

    public ulong TokenId { get; }
}