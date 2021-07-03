using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults

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
