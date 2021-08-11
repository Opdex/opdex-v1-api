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
        public TransactionQuoteRequestResponseModel Request { get; set; }
    }

    public class TransactionQuoteRequestResponseModel
    {
        public string Sender { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
        public string Method { get; set; }
        public IReadOnlyCollection<TransactionQuoteRequestParameterResponseModel> Parameters { get; set; }
        public string Callback { get; set; }
    }

    public class TransactionQuoteRequestParameterResponseModel
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}
