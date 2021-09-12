using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class TransactionLogDto
    {
        public Address Address { get; set; }
        public string[] Topics { get; set; }
        public string Data { get; set; }
        public object Log { get; set; }
        public int SortOrder { get; set; }
    }
}
