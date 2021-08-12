using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    public class CreateTransactionBroadcastCommand : IRequest<string>
    {
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
