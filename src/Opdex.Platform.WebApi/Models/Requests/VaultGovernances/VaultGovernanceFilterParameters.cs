using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

public sealed class VaultGovernanceFilterParameters : FilterParameters<VaultGovernancesCursor>
{
    /// <summary>
    /// Address of the token locked in the vault.
    /// </summary>
    /// <example>tBeY2UAVsbHoS9jwEnT2kMmRsJwUHK1j1L</example>
    public Address LockedToken { get; set; }

    protected override VaultGovernancesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultGovernancesCursor(LockedToken, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        VaultGovernancesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
