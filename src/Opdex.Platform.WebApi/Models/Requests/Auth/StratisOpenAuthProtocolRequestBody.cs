namespace Opdex.Platform.WebApi.Models.Requests.Auth
{
    /// <summary>
    /// Callback body for Stratis Open Auth Protocol
    /// </summary>
    public class StratisOpenAuthProtocolRequestBody
    {
        /// <summary>
        /// Signed Stratis ID callback.
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Message signer wallet address.
        /// </summary>
        public string PublicKey { get; set; }
    }
}