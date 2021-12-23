using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions;

/// <summary>
/// Transaction details.
/// </summary>
public class TransactionResponseModel
{
    public TransactionResponseModel()
    {
        Events = Enumerable.Empty<TransactionEvent>();
    }

    /// <summary>
    /// If the transaction succeeded.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// SHA-256 transaction hash.
    /// </summary>
    /// <example>94ea847222c0ab9f8a16467d4970f389471c0415354b275ec2f4cc9e93e3e95e</example>
    public Sha256 Hash { get; set; }

    /// <summary>
    /// Address of the new smart contract, for a create transaction.
    /// </summary>
    /// <example>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</example>
    public Address NewContractAddress { get; set; }

    /// <summary>
    /// Details for the block that the transaction is included in.
    /// </summary>
    public BlockResponseModel Block { get; set; }

    /// <summary>
    /// Total gas consumed.
    /// </summary>
    /// <example>15000</example>
    [Range(0, double.MaxValue)]
    public int GasUsed { get; set; }

    /// <summary>
    /// Address of the transaction sender.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address From { get; set; }

    /// <summary>
    /// Address of the invoked smart contract, for a call transaction.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address To { get; set; }

    /// <summary>
    /// Transaction events.
    /// </summary>
    public IEnumerable<TransactionEvent> Events { get; set; }
}