using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vault
{
    public class ProcessRevokeVaultCertificateCommandHandler : IRequestHandler<ProcessRevokeVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public ProcessRevokeVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(ProcessRevokeVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeRevokeVaultCertificateCommand(request.WalletName,
                                                                        request.WalletAddress,
                                                                        request.WalletPassword,
                                                                        request.Vault,
                                                                        request.Holder), cancellationToken);
        }
    }
}
