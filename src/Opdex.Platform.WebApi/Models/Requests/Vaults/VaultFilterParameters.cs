using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

public sealed class VaultFilterParameters : FilterParameters<VaultsCursor>
{
    /// <summary>
    /// Address of the token locked in the vault.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address LockedToken { get; set; }

    protected override VaultsCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultsCursor(LockedToken, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        VaultsCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
