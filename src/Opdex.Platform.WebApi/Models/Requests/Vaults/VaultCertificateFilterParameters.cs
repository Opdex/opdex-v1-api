using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    public sealed class VaultCertificateFilterParameters : FilterParameters<VaultCertificatesCursor>
    {
        /// <summary>
        /// Address of the certificate holder.
        /// </summary>
        public Address Holder { get; set; }

        /// <inheritdoc />
        protected override VaultCertificatesCursor InternalBuildCursor()
        {
            if (Cursor is null) return new VaultCertificatesCursor(Holder, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            VaultCertificatesCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
