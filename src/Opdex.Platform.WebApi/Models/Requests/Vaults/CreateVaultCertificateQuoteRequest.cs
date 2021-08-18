using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults
{
    /// <summary>
    /// A request to quote the creation of a vault certificate.
    /// </summary>
    public class CreateVaultCertificateQuoteRequest
    {
        public Address Holder { get; set; }
        public string Amount { get; set; }
    }
}
