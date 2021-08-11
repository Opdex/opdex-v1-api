using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    public class CreateBroadcastTransactionCommand : IRequest<string>
    {
        public CreateBroadcastTransactionCommand(string encodedRequest)
        {
            if (!encodedRequest.HasValue() || !encodedRequest.TryBase64Decode(out string decodedRequest))
            {
                throw new ArgumentException("The encoded request must have a valid value", nameof(encodedRequest));
            }

            QuoteRequest = decodedRequest;
        }

        public string QuoteRequest { get; }
    }
}
