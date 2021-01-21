using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Infrastructure.Abstractions;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents
{
    public class PersistTransactionBurnEventCommandHandler: IRequestHandler<PersistTransactionBurnEventCommand>
    {
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public PersistTransactionBurnEventCommandHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public Task<Unit> Handle(PersistTransactionBurnEventCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}