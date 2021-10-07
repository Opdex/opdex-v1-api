using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.WebApi.Models.Responses;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("vaults")]
    public class VaultsController : ControllerBase
    {
        private readonly IApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public VaultsController(IApplicationContext context, IMapper mapper, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Get Vaults
        /// </summary>
        /// <remarks>Retrieves known vaults</remarks>
        /// <param name="lockedToken">Locked token address.</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="limit">Number of certificates to take must be greater than 0 and less than 51.</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Vaults paging results.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(VaultsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<VaultsResponseModel>> GetVaults([FromQuery] Address lockedToken,
                                                                       [FromQuery] SortDirectionType direction,
                                                                       [FromQuery] uint limit,
                                                                       [FromQuery] string cursor,
                                                                       CancellationToken cancellationToken)
        {
            VaultsCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !VaultsCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new VaultsCursor(lockedToken, direction, limit, PagingDirection.Forward, default);
            }

            var vaults = await _mediator.Send(new GetVaultsWithFilterQuery(pagingCursor), cancellationToken);
            return Ok(_mapper.Map<VaultsResponseModel>(vaults));
        }

        /// <summary>Get Vault</summary>
        /// <remarks>Retrieves vault details for a vault address.</remarks>
        /// <param name="address">Address of the vault.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Vault details</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(VaultResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<VaultResponseModel>> GetVaultByAddress([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new GetVaultByAddressQuery(address), cancellationToken);
            return Ok(_mapper.Map<VaultResponseModel>(vault));
        }

        /// <summary>Set Ownership Quote</summary>
        /// <remarks>Quote a transaction to set the owner of a vault, pending a transaction to redeem ownership.</remarks>
        /// <param name="address">The address of the vault.</param>
        /// <param name="request">Information about the new owner.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpPost("{address}/set-ownership")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> SetOwnerQuote([FromRoute] Address address,
                                                                                     SetVaultOwnerQuoteRequest request,
                                                                                     CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSetPendingVaultOwnershipTransactionQuoteCommand(address, _context.Wallet, request.Owner),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Claim Ownership Quote</summary>
        /// <remarks>Quote a transaction to claim ownership of a vault.</remarks>
        /// <param name="address">The address of the vault.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpPost("{address}/claim-ownership")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> ClaimOwnershipQuote([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateClaimPendingVaultOwnershipTransactionQuoteCommand(address, _context.Wallet),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Get Certificates</summary>
        /// <remarks>Retrieves vault certificates for a vault address</remarks>
        /// <param name="address">Address of the vault</param>
        /// <param name="holder">Certificate holder address</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="limit">Number of certificates to take must be greater than 0 and less than 51.</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Vault certificates</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpGet("{address}/certificates")]
        [ProducesResponseType(typeof(VaultCertificatesResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<VaultCertificatesResponseModel>> GetVaultCertificates([FromRoute] Address address,
                                                                                             [FromQuery] Address holder,
                                                                                             [FromQuery] SortDirectionType direction,
                                                                                             [FromQuery] uint limit,
                                                                                             [FromQuery] string cursor,
                                                                                             CancellationToken cancellationToken)
        {
            VaultCertificatesCursor pagingCursor;

            if (cursor.HasValue())
            {
                if (!Base64Extensions.TryBase64Decode(cursor, out var decodedCursor) || !VaultCertificatesCursor.TryParse(decodedCursor, out var parsedCursor))
                {
                    return new ValidationErrorProblemDetailsResult(nameof(cursor), "Cursor not formed correctly.");
                }
                pagingCursor = parsedCursor;
            }
            else
            {
                pagingCursor = new VaultCertificatesCursor(holder, direction, limit, PagingDirection.Forward, default);
            }

            var certificates = await _mediator.Send(new GetVaultCertificatesWithFilterQuery(address, pagingCursor), cancellationToken);
            return Ok(_mapper.Map<VaultCertificatesResponseModel>(certificates));
        }

        /// <summary>Create Certificate Quote</summary>
        /// <remarks>Quote a transaction to issue a vault certificate.</remarks>
        /// <param name="address">The address of the vault.</param>
        /// <param name="request">Information about the vault certificate that is to be issued.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpPost("{address}/certificates")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CreateCertificateQuote([FromRoute] Address address,
                                                                                              CreateVaultCertificateQuoteRequest request,
                                                                                              CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCreateVaultCertificateTransactionQuoteCommand(address, _context.Wallet, request.Holder, request.Amount),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Redeem Certificates Quote</summary>
        /// <remarks>Quote a transaction to redeem all vested certificates in the vault, that are owned by the authorized address.</remarks>
        /// <param name="address">The address of the vault.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpPost("{address}/certificates/redeem")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> RedeemCertificatesQuote([FromRoute] Address address,
                                                                                               CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateRedeemVaultCertificatesTransactionQuoteCommand(address, _context.Wallet), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Revoke Certificates Quote</summary>
        /// <remarks>Quote a transaction to revoke all the certificates in a vault that are assigned to a particular address.</remarks>
        /// <param name="address">The address of the vault.</param>
        /// <param name="request">Information about the certificate holder.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        /// <response code="404">The vault does not exist.</response>
        [HttpPost("{address}/certificates/revoke")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> RevokeCertificatesQuote([FromRoute] Address address,
                                                                                               RevokeVaultCertificatesQuoteRequest request,
                                                                                               CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateRevokeVaultCertificatesTransactionQuoteCommand(address, _context.Wallet, request.Holder), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
