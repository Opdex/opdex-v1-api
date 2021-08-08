using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Collections.Generic;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Responses;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TransactionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Filter and retrieve Opdex related and indexed transactions.
        /// </summary>
        /// <remarks>
        /// Opdex does not index all smart contract transactions and only watches Opdex receipt logs specifically.
        /// This is not intended to be used to lookup all smart contract based transactions.
        /// </remarks>
        /// <param name="contracts">Optional list of smart contract address to filter transactions by.</param>
        /// <param name="eventTypes">Filter transactions based on event types included.</param>
        /// <param name="wallet">Optionally filter transactions by wallet address.</param>
        /// <param name="limit">Number of transactions to take must be greater than 0 and less than 101.</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionsResponseModel"/> with transactions and paging.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(TransactionsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionsResponseModel>> Transactions([FromQuery] IEnumerable<string> contracts,
                                                                                [FromQuery] IEnumerable<TransactionEventType> eventTypes,
                                                                                [FromQuery] string wallet,
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
                    return new ValidationErrorProblemDetailsResult("Cursor not formed correctly.");
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

        /// <summary>
        /// Retrieve a transaction that has been indexed by its hash.
        /// </summary>
        /// <param name="hash">The transaction hash to of the transaction to look up.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionResponseModel"/> details</returns>
        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(TransactionResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionResponseModel>> Transaction(string hash, CancellationToken cancellationToken)
        {
            var transactionsDto = await _mediator.Send(new GetTransactionByHashQuery(hash), cancellationToken);

            var response = _mapper.Map<TransactionResponseModel>(transactionsDto);

            return Ok(response);
        }
    }
}
