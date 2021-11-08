using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.MarketTokens
{
    public class MarketTokenSnapshotsResponseModel : TokenSnapshotsResponseModel
    {
        /// <summary>
        /// The address of the market.
        /// </summary>
        [NotNull]
        public Address Market { get; set; }
    }
}
