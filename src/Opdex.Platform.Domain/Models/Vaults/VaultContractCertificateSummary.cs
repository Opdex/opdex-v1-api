using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Vaults
{
    public class VaultContractCertificateSummary
    {
        public VaultContractCertificateSummary(UInt256 amount, ulong vestedBlock, bool revoked)
        {
            if (vestedBlock == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vestedBlock), "Vested block must be greater than zero.");
            }

            Amount = amount;
            VestedBlock = vestedBlock;
            Revoked = revoked;
        }

        public UInt256 Amount { get; }
        public ulong VestedBlock { get; }
        public bool Revoked { get; }
    }
}
