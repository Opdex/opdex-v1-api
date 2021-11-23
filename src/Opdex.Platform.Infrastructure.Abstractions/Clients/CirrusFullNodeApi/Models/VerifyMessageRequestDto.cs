using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class VerifyMessageRequestDto
    {
        /// <summary>
        /// Creates a request body for verifying a message on the Cirrus full node.
        /// </summary>
        /// <param name="message">The unsigned message.</param>
        /// <param name="signer">The address of the message signer.</param>
        /// <param name="signature">The signed message.</param>
        public VerifyMessageRequestDto(string message, Address signer, string signature)
        {
            Message = message;
            ExternalAddress = signer;
            Signature = signature;
        }

        public string Message { get; set; }
        public Address ExternalAddress { get; set; }
        public string Signature { get; set; }
    }
}