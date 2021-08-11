using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    public class TransactionQuoteResponseModel
    {
        public object Result { get; set; }
        public string Error { get; set; }
        public uint GasUsed { get; set; }
        public IReadOnlyCollection<TransactionEventResponseModel> Events { get; set; }
        public string Request { get; set; }
    }
}
