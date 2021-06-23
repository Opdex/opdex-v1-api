using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vault;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vault
{
    public class ProcessSetVaultOwnerCommandHandler : IRequestHandler<ProcessSetVaultOwnerCommand, string>
    {
        private readonly IMediator _mediator;

        public ProcessSetVaultOwnerCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(ProcessSetVaultOwnerCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeSetVaultOwnerCommand(request.WalletAddress,
                                                               request.Vault,
                                                               request.Owner), cancellationToken);
        }
    }
}
