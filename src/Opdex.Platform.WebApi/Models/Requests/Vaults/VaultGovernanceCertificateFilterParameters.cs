using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

public sealed class VaultGovernanceCertificateFilterParameters : FilterParameters<VaultGovernanceCertificatesCursor>
{
    /// <summary>
    /// Address of the certificate holder.
    /// </summary>
    /// <example>tBeY2UAVsbHoS9jwEnT2kMmRsJwUHK1j1L</example>
    public Address Holder { get; set; }

    /// <summary>
    /// Status of certificates.
    /// </summary>
    /// <example>All</example>
    public VaultCertificateStatusFilter Status { get; set; }

    protected override VaultGovernanceCertificatesCursor InternalBuildCursor()
    {
        if (EncodedCursor is null) return new VaultGovernanceCertificatesCursor(Holder, Status, Direction, Limit, PagingDirection.Forward, default);
        Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
        VaultGovernanceCertificatesCursor.TryParse(decodedCursor, out var cursor);
        return cursor;
    }
}
