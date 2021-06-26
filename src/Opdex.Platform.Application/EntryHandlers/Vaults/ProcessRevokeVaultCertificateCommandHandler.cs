using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
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
            return _mediator.Send(new MakeRevokeVaultCertificateCommand(request.WalletAddress,
                                                                        request.Vault,
                                                                        request.Holder), cancellationToken);
        }
    }
}
