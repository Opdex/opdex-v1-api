namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class PoolCreatedEventEntity : EventEntityBase
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}