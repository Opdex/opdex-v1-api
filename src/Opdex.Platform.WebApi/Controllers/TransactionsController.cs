using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Linq;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/transactions")]
[ApiVersion("1")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IApplicationContext _context;

    public TransactionsController(IMediator mediator, IMapper mapper, IApplicationContext context)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>Get Transactions</summary>
    /// <remarks>Filter and retrieve indexed Opdex related transactions.</remarks>
    /// <remarks>
    /// Opdex does not index all smart contract transactions and only watches Opdex receipt logs specifically.
    /// This is not intended to be used to lookup all smart contract based transactions.
    /// </remarks>
    /// <param name="filters">Filter parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Details of transactions with paging.</returns>
    [HttpGet]
    public async Task<ActionResult<TransactionsResponseModel>> GetTransactions([FromQuery] TransactionFilterParameters filters,
                                                                               CancellationToken cancellationToken)
    {
        var transactionsDto = await _mediator.Send(new GetTransactionsWithFilterQuery(filters.BuildCursor()), cancellationToken);
        var response = _mapper.Map<TransactionsResponseModel>(transactionsDto);
        return Ok(response);
    }

    /// <summary>Notify Broadcast</summary>
    /// <remarks>Sends notifications to a user about broadcast transactions.</remarks>
    /// <param name="request">The broadcasted transaction details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    public async Task<IActionResult> NotifyBroadcasted([FromBody] TransactionBroadcastNotificationRequest request, CancellationToken cancellationToken)
    {
        var notified = await _mediator.Send(new CreateNotifyUserOfTransactionBroadcastCommand(request.TransactionHash, request.PublicKey), cancellationToken);
        if (!notified) throw new InvalidDataException(nameof(request.TransactionHash), "Invalid transaction state.");
        return NoContent();
    }

    /// <summary>Get Transaction</summary>
    /// <remarks>Retrieve a transaction that has been indexed by its hash.</remarks>
    /// <param name="hash">The SHA256 hash to of the transaction to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Details of the transaction.</returns>
    [HttpGet("{hash}")]
    public async Task<ActionResult<TransactionResponseModel>> GetTransaction([FromRoute] Sha256 hash, CancellationToken cancellationToken)
    {
        var transactionsDto = await _mediator.Send(new GetTransactionByHashQuery(hash), cancellationToken);
        var response = _mapper.Map<TransactionResponseModel>(transactionsDto);
        return Ok(response);
    }

    /// <summary>Replay Transaction Quote</summary>
    /// <remarks>Replay a previous transaction quote to see the current value.</remarks>
    /// <param name="request">A previously quoted request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The replayed transaction quote.</returns>
    [HttpPost("replay-quote")]
    [Authorize]
    public async Task<ActionResult<TransactionQuoteResponseModel>> ReplayTransactionQuote([FromBody] QuoteReplayRequest request, CancellationToken cancellationToken)
    {
        if (_context.Wallet != request.Quote.Sender) throw new NotAllowedException("Transaction quote is not for authenticated wallet");
        var quote = await _mediator.Send(new CreateTransactionQuoteCommand(request.Quote.Sender, request.Quote.To,
                                                                            request.Quote.Amount, request.Quote.Method,
                                                                            request.Quote.Parameters.Select(p => (p.Label, p.Value)),
                                                                            request.Quote.Callback), cancellationToken);
        var response = _mapper.Map<TransactionQuoteResponseModel>(quote);
        return Ok(response);
    }
}
