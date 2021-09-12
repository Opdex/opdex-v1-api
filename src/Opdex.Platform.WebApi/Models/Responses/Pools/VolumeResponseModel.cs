using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class VolumeResponseModel
    {
        public FixedDecimal Crs { get; set; }
        public FixedDecimal Src { get; set; }
        public decimal Usd { get; set; }
    }
}
