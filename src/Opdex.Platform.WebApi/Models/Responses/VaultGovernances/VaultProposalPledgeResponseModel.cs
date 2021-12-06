using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.VaultGovernances;

/// <summary>
/// Vault proposal pledge details.
/// </summary>
public class VaultProposalPledgeResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i</example>
    [NotNull]
    public Address Vault { get; set; }

    /// <summary>
    /// Id of the proposal stored by the vault.
    /// </summary>
    /// <example>5</example>
    [NotNull]
    [Range(1, double.MaxValue)]
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the pledger.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [NotNull]
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
