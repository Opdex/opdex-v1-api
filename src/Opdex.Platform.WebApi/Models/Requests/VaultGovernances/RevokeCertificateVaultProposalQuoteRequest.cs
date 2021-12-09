using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

/// <summary>
/// A request to quote creating a vault proposal for revoking a certificate.
/// </summary>
public class RevokeCertificateVaultProposalQuoteRequest
{
    /// <summary>Address of the certificate owner.</summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Owner { get; set; }

    /// <summary>Details of the proposal. This should provide an explanation of the proposal, with a case for why CRS holders should vote in favor of it.</summary>
    /// <example>OVP-1: Request to revoke certificate. See https://www.example.com for details.</example>
    public string Description { get; set; }
}
