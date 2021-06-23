using Opdex.Platform.WebApi.Models.Responses.OHLC;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class CostResponseModel
    {
        public OhlcBigIntResponseModel CrsPerSrc { get; set; }
        public OhlcBigIntResponseModel SrcPerCrs { get; set; }
    }
}