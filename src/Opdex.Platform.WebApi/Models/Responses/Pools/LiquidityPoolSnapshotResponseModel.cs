using System;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolSnapshotResponseModel : LiquidityPoolSummaryResponseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
