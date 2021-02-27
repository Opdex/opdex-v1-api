using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Transactions;

namespace Opdex.Indexer.Application.Handlers.Transactions
{
    public class RetrieveTransactionByHashQueryHandler : IRequestHandler<RetrieveTransactionByHashQuery, Transaction>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public RetrieveTransactionByHashQueryHandler(IMediator mediator, IMapper mapper, ILogger<RetrieveTransactionByHashQueryHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Transaction> Handle(RetrieveTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectTransactionByHashQuery(request.Hash);
            
            return await _mediator.Send(query, cancellationToken);
        }
    }
}