namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class PairCreatedEventEntity : EventEntityBase
    {
        public string Token { get; set; }
        public string Pair { get; set; }
    }
}