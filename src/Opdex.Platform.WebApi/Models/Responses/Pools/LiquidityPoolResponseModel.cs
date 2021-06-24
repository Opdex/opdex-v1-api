using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolTokenGroupResponseModel
    {
        public TokenResponseModel Crs { get; set; }
        public TokenResponseModel Src { get; set; }
        public TokenResponseModel Lp { get; set; }
        public TokenResponseModel Staking { get; set; }
    }

    public class LiquidityPoolResponseModel : LiquidityPoolSummaryResponseModel
    {
        public string Address { get; set; }
        public LiquidityPoolTokenGroupResponseModel Token { get; set; }
        public MiningPoolResponseModel Mining { get; set; }
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }

    public class LiquidityPoolSnapshotHistoryResponseModel
    {
        public string Address { get; set; }
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
