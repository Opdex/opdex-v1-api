using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.MarketTokens
{
    public class MarketTokenSnapshotsDto : TokenSnapshotsDto
    {
        public Address Market { get; set; }
    }
}
