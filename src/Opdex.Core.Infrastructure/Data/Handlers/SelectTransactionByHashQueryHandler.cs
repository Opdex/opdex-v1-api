using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Core.Infrastructure.Data.Handlers
{
    public class SelectTransactionByHashQueryHandler : IRequestHandler<SelectTransactionByHashQuery, Transaction>
    {
        private readonly IDbContext _context;

        public SelectTransactionByHashQueryHandler(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Transaction> Handle(SelectTransactionByHashQuery request, CancellationToken cancellationToken)
        {
            return new Transaction();
        }
    }
}