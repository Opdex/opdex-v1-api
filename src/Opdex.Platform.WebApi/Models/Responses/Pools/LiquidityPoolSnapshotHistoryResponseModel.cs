using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolSnapshotHistoryResponseModel
    {
        public string Address { get; set; }
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
