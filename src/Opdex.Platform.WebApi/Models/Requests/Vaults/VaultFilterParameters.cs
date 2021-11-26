using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    public sealed class VaultFilterParameters : FilterParameters<VaultsCursor>
    {
        /// <summary>
        /// Address of the locked token.
        /// </summary>
        /// <example>tBeY2UAVsbHoS9jwEnT2kMmRsJwUHK1j1L</example>
        public Address LockedToken { get; set; }

        /// <inheritdoc />
        protected override VaultsCursor InternalBuildCursor()
        {
            if (EncodedCursor is null) return new VaultsCursor(LockedToken, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
            VaultsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
