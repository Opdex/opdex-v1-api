namespace Opdex.Platform.Application.Abstractions.Models.Addresses
{
    public class AddressBalanceDto
    {
        public long Id { get; set; }
        public string Balance { get; set; }

        public string Address { get; set; }

        public string Token { get; set; }
    }
}
