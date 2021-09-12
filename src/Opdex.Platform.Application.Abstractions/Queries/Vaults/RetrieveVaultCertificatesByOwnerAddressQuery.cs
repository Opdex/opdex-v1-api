using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults
{
    public class RetrieveVaultCertificatesByOwnerAddressQuery : IRequest<IEnumerable<VaultCertificate>>
    {
        public RetrieveVaultCertificatesByOwnerAddressQuery(Address ownerAddress)
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
