namespace Opdex.Platform.Common.Enums;

/// <summary>
/// Attribute applied to token's matching values of database lookups.
/// </summary>
public enum TokenAttributeType
{
    /// <summary>
    /// An Opdex Liquidity Pool Token, received for provisioning liquidity in a pool.
    /// </summary>
    Provisional = 1,

    /// <summary>
    /// A unique SRC token, used in liquidity pool pairings.
    /// </summary>
    NonProvisional = 2,

    /// <summary>
    /// Opdex mined, staking governance type tokens.
    /// </summary>
    Staking = 3,

    /// <summary>
    /// Considered a security
    /// </summary>
    Security = 4
}
