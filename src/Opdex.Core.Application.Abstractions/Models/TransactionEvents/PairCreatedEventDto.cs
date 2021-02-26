namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class PairCreatedEventDto : TransactionEventDto
    {
        public string Token { get; set; }
        public string Pair { get; set; }
    }
}