using NJsonSchema.Annotations;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    public class TransactionQuoteResponseModel
    {
        public TransactionQuoteResponseModel()
        {
            Events = new List<TransactionEventResponseModel>().AsReadOnly();
        }

        public object Result { get; set; }

        public string Error { get; set; }

        [NotNull]
        [Range(0, double.MaxValue)]
        public uint GasUsed { get; set; }

        [NotNull]
        public IReadOnlyCollection<TransactionEventResponseModel> Events { get; set; }

        [NotNull]
        public string Request { get; set; }
    }
}
