using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class GetTransactionsWithFilterQueryHandler : EntryFilterQueryHandler<GetTransactionsWithFilterQuery, TransactionsDto>
    {
        private readonly IModelAssembler<Transaction, TransactionDto> _assembler;

        public GetTransactionsWithFilterQueryHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> assembler)
            : base(mediator)
        {
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public override async Task<TransactionsDto> Handle(GetTransactionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _mediator.Send(new RetrieveTransactionsWithFilterQuery(request.Wallet, request.IncludeEvents,
                                                                                            request.ExcludeEvents, request.Contracts,
                                                                                            request.Direction, request.Limit, request.NextParsed,
                                                                                            request.PreviousParsed), cancellationToken);
            // Assemble transaction DTOs
            var transactionDtos = await Task.WhenAll(transactions.Select(transaction => _assembler.Assemble(transaction)));

            // Sort
            var sortedDtos = request.Direction == "ASC"
                ? transactionDtos.OrderBy(t => t.Id).ToList()
                : transactionDtos.OrderByDescending(t => t.Id).ToList();

            // Build the default cursor without next or previous
            var defaultCursor = new StringBuilder($"wallet:{request.Wallet};direction:{request.Direction};limit:{request.Limit};")
                .Append(BuildCursorFromList("includeEvents", request.IncludeEvents.Select(e => e.ToString()).ToList()))
                .Append(BuildCursorFromList("excludeEvents", request.ExcludeEvents.Select(e => e.ToString()).ToList()))
                .Append(BuildCursorFromList("contracts", request.Contracts.Select(c => c.ToString()).ToList()))
                .ToString();

            // Gather first and last values of the response set to build cursors
            var firstCursorValue = sortedDtos.FirstOrDefault()?.Id;
            var lastCursorValue = sortedDtos.LastOrDefault()?.Id;

            (CursorDto cursor, int? removeAt) = BuildCursorDto(request.Previous.HasValue(), request.Next.HasValue(), sortedDtos.Count,
                                                               request.Limit + 1, defaultCursor, firstCursorValue?.ToString(), lastCursorValue?.ToString());

            // Todo: Results are getting messed up because we're supposed to actually remove at before getting the first and last values above.
            if (removeAt.HasValue)
            {
                sortedDtos.RemoveAt(removeAt.Value);
            }

            return new TransactionsDto
            {
                TransactionDtos = sortedDtos,
                Cursor = cursor
            };
        }
    }
}
