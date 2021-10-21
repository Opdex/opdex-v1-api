using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models.Transactions
{
    public class TransactionLogSummaryDto
    {
        public Address Address { get; set; }
        public string[] Topics { get; set; }
        public string Data { get; set; }
        public TransactionLogEventDto Log { get; set; }
        public int SortOrder { get; set; }
    }
}
