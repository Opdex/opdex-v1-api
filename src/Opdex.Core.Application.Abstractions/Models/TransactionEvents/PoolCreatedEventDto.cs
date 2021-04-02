namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class PoolCreatedEventDto : TransactionEventDto
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}