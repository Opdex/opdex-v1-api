using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

/// <summary>
/// Broadcasts a previously quoted transaction.
/// </summary>
public class CreateTransactionBroadcastCommand : IRequest<Sha256>
{
    /// <summary>
    /// Creates a transaction broadcast command from an encoded quoted transaction.
    /// </summary>
    /// <param name="encodedRequest">A previously quoted <see cref="TransactionQuoteRequestDto"/> base64 encoded as a string.</param>
    /// <exception cref="ArgumentNullException">Decoded request null reference</exception>
    public CreateTransactionBroadcastCommand(string encodedRequest)
    {
        if (!encodedRequest.HasValue() || !Base64Extensions.TryBase64Decode(encodedRequest, out var decodedRequest))
        {
            throw new ArgumentException("The encoded request must have a valid value", nameof(encodedRequest));
        }

        QuoteRequest = decodedRequest;
    }

    public string QuoteRequest { get; }
}