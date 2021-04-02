using System;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionEvents
{
    public class TransactionEventSummaryEntity
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public int EventTypeId { get; set; }
        public long EventId { get; set; }
        public string Contract { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}