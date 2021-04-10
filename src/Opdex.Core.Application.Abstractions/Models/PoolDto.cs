namespace Opdex.Core.Application.Abstractions.Models
{
    public class PoolDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public TokenDto Token { get; set; }
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}