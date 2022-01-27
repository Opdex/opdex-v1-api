using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

/// <summary>
/// Replay a transaction quote from a previously base64 encoded <see cref="TransactionQuoteRequestDto"/>
/// </summary>
public class CreateTransactionQuoteCommand : IRequest<TransactionQuoteDto>
{
    /// <summary>
    /// Creates a transaction quote command based on a previous quote request.
    /// </summary>
    /// <exception cref="ArgumentException">Encoded request null reference.</exception>
    public CreateTransactionQuoteCommand(Address sender, Address to, FixedDecimal amount, string method,
                                         IEnumerable<(string, string)> parameters, string callback)
    {
        Sender = sender;
        To = to;
        Amount = amount;
        Method = method;
        Parameters = parameters ?? Enumerable.Empty<(string, string)>();
        Callback = callback;
    }

    public Address Sender { get; }
    public Address To { get; }
    public FixedDecimal Amount { get; }
    public string Method { get; }
    public IEnumerable<(string, string)> Parameters { get; }
    public string Callback { get; }
}
