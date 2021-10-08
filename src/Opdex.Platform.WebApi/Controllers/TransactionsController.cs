using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Collections.Generic;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Exceptions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;
        private readonly NetworkType _network;

        public TransactionsController(IMediator mediator, IMapper mapper, IApplicationContext context, OpdexConfiguration opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        /// <summary>Get Transactions</summary>
        /// <remarks>Filter and retrieve Opdex related and indexed transactions.</remarks>
        /// <remarks>
        /// Opdex does not index all smart contract transactions and only watches Opdex receipt logs specifically.
        /// This is not intended to be used to lookup all smart contract based transactions.
        /// </remarks>
        /// <param name="contracts">Optional list of smart contract address to filter transactions by.</param>
        /// <param name="eventTypes">Filter transactions based on event types included.</param>
        /// <param name="wallet">Optionally filter transactions by wallet address.</param>
        /// <param name="limit">Number of transactions to take must be greater than 0 and less than 51.</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionsResponseModel"/> with transactions and paging.</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(TransactionsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionsResponseModel>> Transactions([FromQuery] IEnumerable<Address> contracts,
                                                                                [FromQuery] IEnumerable<TransactionEventType> eventTypes,
                                                                                [FromQuery] Address wallet,
                                                                                [FromQuery] uint limit,
                                                                                [FromQuery] SortDirectionType direction,
                                                                                [FromQuery] string cursor,
                                                                                CancellationToken cancellationToken)
        {
            TransactionsCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !TransactionsCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new TransactionsCursor(wallet, eventTypes, contracts, direction, limit, PagingDirection.Forward, 0);
            }

            var transactionsDto = await _mediator.Send(new GetTransactionsWithFilterQuery(pagingCursor), cancellationToken);

            var response = _mapper.Map<TransactionsResponseModel>(transactionsDto);

            return Ok(response);
        }

        /// <summary>Notify Broadcast</summary>
        /// <remarks>Sends notifications to a user about broadcast transactions.</remarks>
        /// <param name="request">The broadcasted transaction details.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> NotifyBroadcasted(TransactionBroadcastNotificationRequest request, CancellationToken cancellationToken)
        {
            var notified = await _mediator.Send(new CreateNotifyUserOfTransactionBroadcastCommand(request.WalletAddress, request.TransactionHash), cancellationToken);
            if (!notified) throw new InvalidDataException(nameof(request.TransactionHash), "Transaction could not be found in the mempool.");
            return NoContent();
        }

        /// <summary>Get Transaction</summary>
        /// <remarks>Retrieve a transaction that has been indexed by its hash.</remarks>
        /// <param name="hash">The transaction hash to of the transaction to look up.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionResponseModel"/> details</returns>
        [HttpGet("{hash}")]
        [Authorize]
        [ProducesResponseType(typeof(TransactionResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionResponseModel>> Transaction(string hash, CancellationToken cancellationToken)
        {
            var transactionsDto = await _mediator.Send(new GetTransactionByHashQuery(hash), cancellationToken);

            var response = _mapper.Map<TransactionResponseModel>(transactionsDto);

            return Ok(response);
        }

        /// <summary>Broadcast Transaction Quote - Devnet Only</summary>
        /// <remarks>Broadcast a previously quoted transaction. Network dependent, for devnet use only.</remarks>
        /// <param name="request">The quoted transaction to broadcast.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Transaction hash and sender address.</returns>
        [HttpPost("broadcast-quote")]
        [Authorize]
        [ProducesResponseType(typeof(BroadcastTransactionResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BroadcastTransactionResponseModel>> BroadcastTransactionQuote(QuoteReplayRequest request, CancellationToken cancellationToken)
        {
            // Todo: Reusable Network Attribute - Devnet Only; Else 404
            if (_network != NetworkType.DEVNET)
            {
                return new NotFoundResult();
            }

            if (!request.Quote.HasValue() || !request.Quote.TryBase64Decode(out string decodedRequest))
            {
                return new ValidationErrorProblemDetailsResult(nameof(request.Quote), "Quote not formed correctly.");
            }

            var txHash = await _mediator.Send(new CreateTransactionBroadcastCommand(decodedRequest), cancellationToken);

            return Ok(new BroadcastTransactionResponseModel { TxHash = txHash, Sender = _context.Wallet });
        }

        /// <summary>Replay Transaction Quote</summary>
        /// <remarks>Replay a previous transaction quote to see the current value.</remarks>
        /// <param name="request">A previously quoted request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> outcome of the quote.</returns>
        [HttpPost("replay-quote")]
        [Authorize]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> ReplayTransactionQuote(QuoteReplayRequest request, CancellationToken cancellationToken)
        {
            var quote = await _mediator.Send(new CreateTransactionQuoteCommand(request.Quote), cancellationToken);

            var response = _mapper.Map<TransactionQuoteResponseModel>(quote);

            return Ok(response);
        }
    }
}
