using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Transactions
{
    public class TransactionQuoteRequestDto
    {
        public TransactionQuoteRequestDto()
        {
            Parameters = new List<TransactionQuoteRequestParameterDto>().AsReadOnly();
        }

        public Address Sender { get; set; }
        public Address To { get; set; }
        public string Amount { get; set; }
        public string Method { get; set; }
        public IReadOnlyCollection<TransactionQuoteRequestParameterDto> Parameters { get; set; }
        public string Callback { get; set; }
    }
}
