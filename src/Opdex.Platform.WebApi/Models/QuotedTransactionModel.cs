using Opdex.Platform.Common.Models;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models;

public class QuotedTransactionModel
{
    public Address Sender { get; set; }
    public Address To { get; set; }
    public FixedDecimal Amount { get; set; }
    public string Method { get; set; }
    public IReadOnlyCollection<TransactionParameterModel> Parameters { get; set; }
    public string Callback { get; set; }
}

public class TransactionParameterModel
{
    public string Label { get; set; }
    public string Value { get; set; }
}
