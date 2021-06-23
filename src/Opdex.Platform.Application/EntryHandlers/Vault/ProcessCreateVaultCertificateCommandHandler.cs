using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vault
{
    public class ProcessCreateVaultCertificateCommandHandler : IRequestHandler<ProcessCreateVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public ProcessCreateVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(ProcessCreateVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeCreateVaultCertificateCommand(request.WalletName,
                                                                        request.WalletAddress,
                                                                        request.WalletPassword,
                                                                        request.Vault,
                                                                        request.Holder,
                                                                        request.Amount), cancellationToken);
        }
    }
}
