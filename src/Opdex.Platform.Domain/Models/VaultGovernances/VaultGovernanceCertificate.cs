using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

/// <summary>
/// Entitles the certificate holder to an amount of Opdex governance tokens.
/// </summary>
public class VaultGovernanceCertificate : BlockAudit
{
    public VaultGovernanceCertificate(ulong vaultGovernanceId, Address owner, UInt256 amount, ulong vestedBlock, ulong createdBlock) : base(createdBlock)
    {
        if (vaultGovernanceId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(vaultGovernanceId), "Vault governance id must be greater than 0.");
        }

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner must be set.");
        }

        if (vestedBlock < 1)
        {
            throw new ArgumentNullException(nameof(vestedBlock), "Vested block must be greater than 0.");
        }

        if (amount == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
        }

        VaultGovernanceId = vaultGovernanceId;
        Owner = owner;
        Amount = amount;
        VestedBlock = vestedBlock;
        Redeemed = false;
        Revoked = false;
    }

    public VaultGovernanceCertificate(ulong id, ulong vaultGovernanceId, Address owner, UInt256 amount, ulong vestedBlock, bool redeemed, bool revoked,
                                      ulong createdBlock, ulong modifiedBlock)
        : base(createdBlock, modifiedBlock)
    {
        Id = id;
        VaultGovernanceId = vaultGovernanceId;
        Owner = owner;
        Amount = amount;
        VestedBlock = vestedBlock;
        Redeemed = redeemed;
        Revoked = revoked;
    }

    public ulong Id { get; }
    public ulong VaultGovernanceId { get; }
    public Address Owner { get; }
    public UInt256 Amount { get; private set; }
    public bool Revoked { get; private set; }
    public ulong VestedBlock { get; }
    public bool Redeemed { get; private set; }

    public void Revoke(RevokeVaultCertificateLog log, ulong block)
    {
        Amount = log.NewAmount;
        Revoked = true;
        SetModifiedBlock(block);
    }

    public void Redeem(RedeemVaultCertificateLog log, ulong block)
    {
        Redeemed = true;
        SetModifiedBlock(block);
    }

    public void Update(VaultContractCertificateSummary summary, ulong blockHeight)
    {
        // Summaries are only returned for non-redeemed certs
        Redeemed = false;
        Amount = summary.Amount;
        Revoked = summary.Revoked;
        SetModifiedBlock(blockHeight);
    }
}
