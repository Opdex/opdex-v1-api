using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault
{
    public class SelectVaultCertificatesByOwnerAddressQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public SelectVaultCertificatesByOwnerAddressQuery(string ownerAddress)
        {
            if (!ownerAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(ownerAddress));
            }
            
            OwnerAddress = ownerAddress;
        }
        
        public string OwnerAddress { get; }
    }
}