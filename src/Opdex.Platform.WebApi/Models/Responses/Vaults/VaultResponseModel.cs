using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults;

/// <summary>
/// Vault details.
/// </summary>
public class VaultResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i</example>
    public Address Address { get; set; }

    /// <summary>
    /// Address of the pending owner.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address PendingOwner { get; set; }

    /// <summary>
    /// Address of the current owner.
    /// </summary>
    /// <example>tHYHem7cLKgoLkeb792yn4WayqKzLrjJak</example>
    public Address Owner { get; set; }

    /// <summary>
    /// The block which the vault was created.
    /// </summary>
    /// <example>500000</example>
    [Range(1, double.MaxValue)]
    public ulong Genesis { get; set; }

    /// <summary>
    /// The total number of tokens locked in the vault.
    /// </summary>
    /// <example>"250000000.00000000"</example>
    public FixedDecimal TokensLocked { get; set; }

    /// <summary>
    /// The total number of tokens locked in the vault, that are not assigned to any address.
    /// </summary>
    /// <example>"200000000.00000000"</example>
    public FixedDecimal TokensUnassigned { get; set; }

    /// <summary>
    /// Address of the token locked in the vault.
    /// </summary>
    /// <example>tBeY2UAVsbHoS9jwEnT2kMmRsJwUHK1j1L</example>
    public Address LockedToken { get; set; }
}