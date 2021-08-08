using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotHistoryResponseModel
    {
        public string Address { get; set; }
        public IEnumerable<TokenSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
