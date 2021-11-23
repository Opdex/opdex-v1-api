using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth
{
    /// <summary>
    /// Request to call Cirrus full node to verify a signed message.
    /// </summary>
    public class CallCirrusVerifyMessageQuery : IRequest<bool>
    {
        /// <summary>
        /// Creates a request to call Cirrus full node to verify a signer message.
        /// </summary>
        /// <param name="message">The unsigned message.</param>
        /// <param name="signer">The address of the message signer.</param>
        /// <param name="signature">The signed message.</param>
        public CallCirrusVerifyMessageQuery(string message, Address signer, string signature)
        {
            if (!message.HasValue()) throw new ArgumentNullException("Message must not be empty.");
            if (signer == Address.Empty) throw new ArgumentNullException("Signer must not be empty.");
            if (!signature.HasValue()) throw new ArgumentNullException("Signature must not be empty.");
            Message = message;
            Signer = signer;
            Signature = signature;
        }

        public string Message { get; }
        public Address Signer { get; }
        public string Signature { get; }
    }
}