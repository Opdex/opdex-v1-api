using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults
{
    public class PersistVaultCertificateCommand : IRequest<bool>
    {
        public PersistVaultCertificateCommand(VaultCertificate vaultCertificate)
        {
            if (vaultCertificate == null)
            {
                VaultCertificate = vaultCertificate;
            }
            
            VaultCertificate = vaultCertificate;
        }
        
        public VaultCertificate VaultCertificate { get; }
    }
}
