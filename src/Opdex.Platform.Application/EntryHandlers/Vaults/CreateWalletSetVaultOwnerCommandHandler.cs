using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateWalletSetVaultOwnerCommandHandler : IRequestHandler<CreateWalletSetVaultOwnerCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletSetVaultOwnerCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<string> Handle(CreateWalletSetVaultOwnerCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new MakeWalletSetVaultOwnerCommand(request.WalletAddress, request.Vault, request.Owner));
        }
    }
}
