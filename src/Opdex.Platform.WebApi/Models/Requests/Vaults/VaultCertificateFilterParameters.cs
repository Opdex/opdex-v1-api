using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

public sealed class VaultCertificateFilterParameters : FilterParameters<VaultCertificatesCursor>
{
    /// <summary>
    /// Address of the certificate owner.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address Owner { get; set; }

    /// <summary>
    /// Status of certificates.
    /// </summary>
    /// <example>All</example>
    public VaultCertificateStatusFilter Status { get; set; }

    protected override VaultCertificatesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultCertificatesCursor(Owner, Status, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        VaultCertificatesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
