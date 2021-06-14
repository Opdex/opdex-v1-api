namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class CostDto
    {
        public OhlcDto CrsPerSrc { get; set; }
        public OhlcDto SrcPerCrs { get; set; }
    }
}