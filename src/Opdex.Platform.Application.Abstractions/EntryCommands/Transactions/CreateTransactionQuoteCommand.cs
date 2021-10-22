using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    /// <summary>
    /// Replay a transaction quote from a previously base64 encoded <see cref="TransactionQuoteRequestDto"/>
    /// </summary>
    public class CreateTransactionQuoteCommand : IRequest<TransactionQuoteDto>
    {
        /// <summary>
        /// Creates a transaction quote command based on a previous quote request.
        /// </summary>
        /// <param name="encodedRequest">A previously quoted <see cref="TransactionQuoteRequestDto"/> base64 encoded as a string.</param>
        /// <exception cref="ArgumentException">Encoded request null reference.</exception>
        public CreateTransactionQuoteCommand(string encodedRequest)
        {
            if (!encodedRequest.HasValue() || !Base64Extensions.TryBase64Decode(encodedRequest, out var decodedRequest))
            {
                throw new ArgumentException("The encoded request must have a valid value", nameof(encodedRequest));
            }

            QuoteRequest = decodedRequest;
        }

        public string QuoteRequest { get; }
    }
}
