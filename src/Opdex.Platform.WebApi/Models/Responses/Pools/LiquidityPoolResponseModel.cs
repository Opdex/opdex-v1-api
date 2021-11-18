using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolResponseModel : LiquidityPoolSummaryResponseModel
    {
        public Address Address { get; set; }

        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TransactionFee { get; set; }

        public LiquidityPoolTokenGroupResponseModel Token { get; set; }

        public MiningPoolResponseModel Mining { get; set; }
        
        public IEnumerable<LiquidityPoolSnapshotResponseModel> SnapshotHistory { get; set; }
    }
}
