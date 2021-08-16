using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    /// <summary>
    /// Broadcasts a previously quoted transaction.
    /// </summary>
    public class CreateTransactionBroadcastCommand : IRequest<string>
    {
        /// <summary>
        /// Creates a transaction broadcast command from an encoded quoted transaction.
        /// </summary>
        /// <param name="decodedRequest">The decoded JSON string of the previously quoted transaction.</param>
        /// <exception cref="ArgumentNullException">Decoded request null reference</exception>
        public CreateTransactionBroadcastCommand(string decodedRequest)
        {
            if (!decodedRequest.HasValue())
            {
                throw new ArgumentNullException(nameof(decodedRequest), "The decoded request must have a valid value");
            }

            QuoteRequest = decodedRequest;
        }

        public string QuoteRequest { get; }
    }
}
