using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;

namespace Opdex.Platform.Application.Handlers.vault

{
    public class MakeVaultCommandHandler : IRequestHandler<MakeVaultCommand, long>
    {
        private readonly IMediator _mediator;
        
        public MakeVaultCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<long> Handle(MakeVaultCommand request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new PersistVaultCommand(request.Vault), cancellationToken);
        }
    }
}