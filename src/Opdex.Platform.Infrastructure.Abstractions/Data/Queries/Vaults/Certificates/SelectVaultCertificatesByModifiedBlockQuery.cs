using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates
{
    public class SelectVaultCertificatesByModifiedBlockQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public SelectVaultCertificatesByModifiedBlockQuery(ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
    }
}
