using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions;

public class GetTransactionsWithFilterQueryHandler : EntryFilterQueryHandler<GetTransactionsWithFilterQuery, TransactionsDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<Transaction, TransactionDto> _assembler;
    private readonly ILogger<GetTransactionsWithFilterQueryHandler> _logger;

    public GetTransactionsWithFilterQueryHandler(IMediator mediator, IModelAssembler<Transaction, TransactionDto> assembler, ILogger<GetTransactionsWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<TransactionsDto> Handle(GetTransactionsWithFilterQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _mediator.Send(new RetrieveTransactionsWithFilterQuery(request.Cursor), cancellationToken);

        var transactionResults = transactions.ToList();

        _logger.LogTrace("Retrieved queried transactions");

        var cursor = BuildCursorDto(transactionResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", transactionResults.Count);

        var dtos = await Task.WhenAll(transactionResults.Select(transaction => _assembler.Assemble(transaction)));

        _logger.LogTrace("Assembled results");

        return new TransactionsDto { Transactions = dtos, Cursor = cursor };
    }
}
