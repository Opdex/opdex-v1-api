using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class VaultCertificate : BlockAudit
    {
        public VaultCertificate(long vaultId, string owner, string amount, ulong vestedBlock, ulong createdBlock) : base(createdBlock)
        {
            if (vaultId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vaultId), "Vault id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount), "Amount must only contain numeric digits.");
            }

            if (vestedBlock < 1)
            {
                throw new ArgumentNullException(nameof(vestedBlock), "Vested block must be greater than 0.");
            }

            VaultId = vaultId;
            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
            Redeemed = false;
            Revoked = false;
        }

        public VaultCertificate(long id, long vaultId, string owner, string amount, ulong vestedBlock, bool redeemed, bool revoked, ulong createdBlock, ulong modifiedBlock)
            : base(createdBlock, modifiedBlock)
        {
            Id = id;
            VaultId = vaultId;
            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
            Redeemed = redeemed;
            Revoked = revoked;
        }

        public long Id { get; }
        public long VaultId { get; }
        public string Owner { get; }
        public string Amount { get; private set; }
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
    }
}