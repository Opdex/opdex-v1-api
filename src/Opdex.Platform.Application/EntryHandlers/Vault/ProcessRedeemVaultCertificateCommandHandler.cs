using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vault
{
    public class ProcessRedeemVaultCertificateCommandHandler : IRequestHandler<ProcessRedeemVaultCertificateCommand, string>
    {
        private readonly IMediator _mediator;

        public ProcessRedeemVaultCertificateCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(ProcessRedeemVaultCertificateCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeRedeemVaultCertificateCommand(request.WalletAddress,
                                                                        request.Vault,
                                                                        request.Holder), cancellationToken);
        }
    }
}
