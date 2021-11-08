using Opdex.Platform.Common.Models;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotsResponseModel : PaginatedResponseModel<TokenSnapshotResponseModel>
    {
        /// <summary>
        /// The address of the token, or CRS.
        /// </summary>
        [NotNull]
        public Address Token { get; set; }
    }
}
