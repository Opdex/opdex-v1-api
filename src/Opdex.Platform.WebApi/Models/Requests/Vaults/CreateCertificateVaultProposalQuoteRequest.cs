using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Vaults;

/// <summary>
/// A request to quote creating a vault proposal for creating a certificate.
/// </summary>
public class CreateCertificateVaultProposalQuoteRequest
{
    /// <summary>Address of the certificate owner.</summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Owner { get; set; }

    /// <summary>Amount of governance tokens to assign to the certificate.</summary>
    /// <example>"2500000.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>Details of the proposal. This should provide an explanation of the proposal, with a case for why CRS holders should vote in favor of it.</summary>
    /// <example>OVP-1: Request to create certificate. See https://www.example.com for details.</example>
    public string Description { get; set; }
}
