namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class CostResponseModel
    {
        public OhlcResponseModel CrsPerSrc { get; set; }
        public OhlcResponseModel SrcPerCrs { get; set; }
    }
}