using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
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
            return _mediator.Send(new MakeCreateVaultCertificateCommand(request.WalletAddress,
                                                                        request.Vault,
                                                                        request.Holder,
                                                                        request.Amount), cancellationToken);
        }
    }
}
