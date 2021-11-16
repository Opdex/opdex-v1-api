using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;

namespace Opdex.Platform.WebApi.Models.Requests.Governances
{
    public sealed class GovernanceFilterParameters : FilterParameters<MiningGovernancesCursor>
    {
        /// <summary>
        /// Address of the mined token.
        /// </summary>
        public Address MinedToken { get; set; }

        /// <inheritdoc />
        protected override MiningGovernancesCursor InternalBuildCursor()
        {
            if (EncodedCursor is null) return new MiningGovernancesCursor(MinedToken, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(EncodedCursor, out var decodedCursor);
            MiningGovernancesCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
