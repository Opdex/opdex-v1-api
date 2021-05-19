using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Domain.Models.ODX
{
    public class VaultCertificate
    {
        public VaultCertificate(long vaultId, string owner, string amount, ulong vestedBlock, ulong createdBlock)
        {
            if (vaultId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(vaultId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException();
            }

            if (!amount.IsNumeric())
            {
                throw new ArgumentNullException(nameof(amount));
            }

            if (vestedBlock < 1)
            {
                throw new ArgumentNullException(nameof(vestedBlock));
            }
            
            if (createdBlock < 1)
            {
                throw new ArgumentNullException(nameof(createdBlock));
            }
            
            VaultId = vaultId;
            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
            Redeemed = false;
            CreatedBlock = createdBlock;
            ModifiedBlock = createdBlock;
        }
        
        public VaultCertificate(long id, long vaultId, string owner, string amount, ulong vestedBlock, bool redeemed, ulong createdBlock, ulong modifiedBlock)
        {
            Id = id;
            VaultId = vaultId;
            Owner = owner;
            Amount = amount;
            VestedBlock = vestedBlock;
            Redeemed = redeemed;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }

        public long Id { get; }
        public long VaultId { get; }
        public string Owner { get; }
        public string Amount { get; private set; }
        public ulong VestedBlock { get; }
        public bool Redeemed { get; private set; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }

        public void UpdateAmount(RevokeVaultCertificateLog log)
        {
            if (log.Owner != Owner)
            {
                throw new ArgumentException($"Log owner {log.Owner} is not the certificate owner {Owner}.");
            }
            
            Amount = log.NewAmount;
        }
        
        public void Redeem(RedeemVaultCertificateLog log)
        {
            if (log.Owner != Owner)
            {
                throw new ArgumentException($"Log owner {log.Owner} is not the certificate owner {Owner}.");
            }
            
            if (log.Amount != Amount)
            {
                throw new ArgumentException($"Log amount {log.Amount} must match the certificate amount {Amount}.");
            }
            
            Redeemed = true;
        }
    }
}