using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults;

/// <summary>
/// Vault proposal pledge details.
/// </summary>
public class VaultProposalPledgeResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G</example>
    public Address Vault { get; set; }

    /// <summary>
    /// Id of the proposal stored by the vault.
    /// </summary>
    /// <example>5</example>
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the pledger.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Pledger { get; set; }

    /// <summary>
    /// Amount of CRS pledged to the proposal. This will not change once the pledge period has finished.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Pledge { get; set; }

    /// <summary>
    /// Currently pledged CRS balance.
    /// </summary>
    /// <example>"0.00000000"</example>
    public FixedDecimal Balance { get; set; }
}
