namespace Opdex.Platform.WebApi.Models
{
    public class PoolResponseModel
    {
        public string Address { get; set; }
        
        // Todo: Should use assembler to get the full token details
        public long TokenId { get; set; }
        
        // Todo: Should move to a PoolSnapshot model
        public ulong ReserveCrs { get; set; }
        
        public string ReserveSrc { get; set; }
    }
}