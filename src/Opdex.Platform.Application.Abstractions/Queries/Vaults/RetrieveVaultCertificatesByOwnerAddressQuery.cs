using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.ODX;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
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
