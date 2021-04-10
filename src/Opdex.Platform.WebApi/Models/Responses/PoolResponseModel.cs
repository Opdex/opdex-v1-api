namespace Opdex.Platform.WebApi.Models
{
    public class PoolResponseModel
    {
        public string Address { get; set; }
        
        public TokenResponseModel Token { get; set; }
        
        // Todo: Should move to a PoolSnapshot model
        public ulong ReserveCrs { get; set; }
        
        public string ReserveSrc { get; set; }
    }
}