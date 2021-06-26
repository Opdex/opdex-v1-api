using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
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
            return _mediator.Send(new MakeSetVaultOwnerCommand(request.WalletAddress, request.Vault, request.Owner));
        }
    }
}
