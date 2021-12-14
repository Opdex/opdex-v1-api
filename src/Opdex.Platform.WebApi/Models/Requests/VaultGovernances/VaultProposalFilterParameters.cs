using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

public class VaultProposalFilterParameters : FilterParameters<VaultProposalsCursor>
{
    /// <summary>
    /// Status of the proposal.
    /// </summary>
    /// <example>Vote</example>
    public VaultProposalStatus Status { get; set; }

    /// <summary>
    /// Type of proposal.
    /// </summary>
    /// <example>Create</example>
    public VaultProposalType Type { get; set; }

    /// <inheritdoc />
    protected override VaultProposalsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultProposalsCursor(Status, Type, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        _ = VaultProposalsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}