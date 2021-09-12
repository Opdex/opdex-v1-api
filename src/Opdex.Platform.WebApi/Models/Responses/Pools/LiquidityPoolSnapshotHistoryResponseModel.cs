using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolSnapshotHistoryResponseModel
    {
        public Address Address { get; set; }
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
