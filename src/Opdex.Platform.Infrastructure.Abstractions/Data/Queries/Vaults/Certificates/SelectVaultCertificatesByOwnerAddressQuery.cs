using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates
{
    public class SelectVaultCertificatesByOwnerAddressQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public SelectVaultCertificatesByOwnerAddressQuery(Address ownerAddress)
        {
            if (ownerAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(ownerAddress));
            }

            OwnerAddress = ownerAddress;
        }

        public Address OwnerAddress { get; }
    }
}
