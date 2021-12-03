using Opdex.Platform.Common.Models;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions;

public class BroadcastTransactionResponseModel
{
    /// <summary>
    /// SHA-256 transaction hash.
    /// </summary>
    /// <value>402aa2241adb7b04d07d4dbc89f8aae72fa1c11f9bd2bd9013222cd774ed39fe</value>
    [NotNull]
    public Sha256 TxHash { get; set; }

    /// <summary>
    /// Address of the transaction sender.
    /// </summary>
    /// <value>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</value>
    [NotNull]
    public Address Sender { get; set; }
}