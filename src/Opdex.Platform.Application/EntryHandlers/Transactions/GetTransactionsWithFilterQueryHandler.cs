using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions
{
    public class GetTransactionsWithFilterQueryHandler : EntryFilterQueryHandler<GetTransactionsWithFilterQuery, TransactionsDto>
    {
        private readonly IModelAssembler<Transaction, TransactionDto> _assembler;
        private const string PagingBase = "wallet:{0};direction:{1};limit:{2};<eventTypes><contracts>";

        public GetTransactionsWithFilterQueryHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> assembler)
            : base(mediator)
        {
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public override async Task<TransactionsDto> Handle(GetTransactionsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _mediator.Send(new RetrieveTransactionsWithFilterQuery(request.Wallet, request.EventTypes, request.Contracts,
                                                                                            request.Direction, request.Limit, request.Next,
                                                                                            request.Previous), cancellationToken);
            // Assemble transaction DTOs
            var transactionDtos = await Task.WhenAll(transactions.Select(transaction => _assembler.Assemble(transaction)));

            // Sort
            var sortedDtos = request.Direction == SortDirectionType.ASC
                ? transactionDtos.OrderBy(t => t.Id).ToList()
                : transactionDtos.OrderByDescending(t => t.Id).ToList();

            // Build the default cursor without next or previous
            var baseCursor = string.Format(PagingBase, request.Wallet, request.Direction, request.Limit)
                .Replace("<eventTypes>", BuildCursorFromList("eventTypes", request.EventTypes.Select(e => e.ToString())))
                .Replace("<contracts>", BuildCursorFromList("contracts", request.Contracts.Select(c => c)));

            // The count can change if we remove the + 1 record, we want the original
            var sortedDtosCount = sortedDtos.Count;
            var limitPlusOne = request.Limit + 1;

            // Remove the + 1 value if necessary
            var removeAtIndex = RemoveAtIndex(request.PagingBackward, sortedDtosCount, limitPlusOne);
            if (removeAtIndex.HasValue)
            {
                sortedDtos.RemoveAt(removeAtIndex.Value);
            }

            // Gather first and last values of the response set to build cursors after the + 1 has been removed.
            var firstCursorValue = sortedDtos.FirstOrDefault()?.Id.ToString();
            var lastCursorValue = sortedDtos.LastOrDefault()?.Id.ToString();

            // Build the cursor DTO
            var cursor = BuildCursorDto(request.PagingBackward, request.PagingForward, sortedDtosCount,
                                        limitPlusOne, baseCursor, firstCursorValue, lastCursorValue);

            return new TransactionsDto { TransactionDtos = sortedDtos, CursorDto = cursor };
        }
    }
}
