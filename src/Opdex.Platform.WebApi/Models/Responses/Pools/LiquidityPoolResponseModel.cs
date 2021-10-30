using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolResponseModel : LiquidityPoolSummaryResponseModel
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public decimal TransactionFee { get; set; }
        public LiquidityPoolTokenGroupResponseModel Token { get; set; }
        public MiningPoolResponseModel Mining { get; set; }
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
