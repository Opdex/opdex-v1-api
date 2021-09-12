using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotHistoryResponseModel
    {
        public Address Address { get; set; }
        public IEnumerable<TokenSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
