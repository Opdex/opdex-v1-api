using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Transactions
{
    public class TransactionQuoteRequestDto
    {
        public TransactionQuoteRequestDto()
        {
            Parameters = new List<TransactionQuoteRequestParameterDto>();
        }

        public string Sender { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
        public string Method { get; set; }
        public IReadOnlyCollection<TransactionQuoteRequestParameterDto> Parameters { get; set; }
        public string Callback { get; set; }
    }
}
