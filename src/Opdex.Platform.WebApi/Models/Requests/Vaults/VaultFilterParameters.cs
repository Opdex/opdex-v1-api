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
        public Address LockedToken { get; set; }

        /// <inheritdoc />
        protected override VaultsCursor InternalBuildCursor()
        {
            if (Cursor is null) return new VaultsCursor(LockedToken, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            VaultsCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
