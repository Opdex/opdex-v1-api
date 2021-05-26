using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Queries.Vault
{
    public class RetrieveVaultCertificatesByOwnerAddressQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public RetrieveVaultCertificatesByOwnerAddressQuery(string ownerAddress)
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