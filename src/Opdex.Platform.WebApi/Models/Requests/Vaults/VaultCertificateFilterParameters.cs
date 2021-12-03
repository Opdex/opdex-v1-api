using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

public sealed class VaultCertificateFilterParameters : FilterParameters<VaultCertificatesCursor>
{
    /// <summary>
    /// Address of the certificate holder.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Holder { get; set; }

    /// <inheritdoc />
    protected override VaultCertificatesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultCertificatesCursor(Holder, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        VaultCertificatesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}