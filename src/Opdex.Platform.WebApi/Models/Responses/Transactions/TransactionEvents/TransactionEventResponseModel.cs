using Opdex.Platform.Common.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    public abstract class TransactionEventResponseModel
    {
        public TransactionEventType EventType { get; set; }

        [NotNull]
        public string Contract { get; set; }

        public int SortOrder { get; set; }
    }
}
