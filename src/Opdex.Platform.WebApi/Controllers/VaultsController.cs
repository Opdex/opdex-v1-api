using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
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

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("vaults")]
    public class VaultsController : ControllerBase
    {
        private readonly IApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly BlockExplorerConfiguration _blockExplorerConfig;

        public VaultsController(IApplicationContext context, IMapper mapper, IMediator mediator, BlockExplorerConfiguration blockExplorerConfig)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _blockExplorerConfig = blockExplorerConfig ?? throw new ArgumentNullException(nameof(blockExplorerConfig));
        }

        /// <summary>
        /// Get Vaults
        /// </summary>
        /// <remarks>Retrieves known vaults</remarks>
        /// <param name="lockedToken">Locked token address.</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="limit">Number of certificates to take must be greater than 0 and less than 101.</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Vaults paging results</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VaultsResponseModel>> GetVaults([FromQuery] string lockedToken,
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
                    return new ValidationErrorProblemDetailsResult("Cursor not formed correctly.");
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
        /// <remarks>Retrieves vault details for a vault address</remarks>
        /// <param name="address">Address of the vault</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Vault details</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VaultResponseModel>> GetVaultByAddress(string address, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new GetVaultByAddressQuery(address), cancellationToken);
            return Ok(_mapper.Map<VaultResponseModel>(vault));
        }

        /// <summary>Set Owner Quote</summary>
        /// <remarks>Sets the owner of a vault</remarks>
        /// <param name="address">Vault contract address</param>
        /// <param name="request">Set owner request.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Cirrus transaction hash</returns>
        [HttpPost("{address}/owner")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> SetOwner(string address,
                                                         SetVaultOwnerRequest request,
                                                         CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new CreateWalletSetVaultOwnerCommand(_context.Wallet,
                                                                                            address,
                                                                                            request.Owner), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>Get Certificates</summary>
        /// <remarks>Retrieves vault certificates for a vault address</remarks>
        /// <param name="address">Address of the vault</param>
        /// <param name="holder">Certificate holder address</param>
        /// <param name="direction">The order direction of the results, either "ASC" or "DESC".</param>
        /// <param name="limit">Number of certificates to take must be greater than 0 and less than 101.</param>
        /// <param name="cursor">The cursor when paging.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Vault certificates</returns>
        [HttpGet("{address}/certificates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<VaultCertificatesResponseModel>> GetVaultCertificates(string address,
                                                                                             [FromQuery] string holder,
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
                    return new ValidationErrorProblemDetailsResult("Cursor not formed correctly.");
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
        /// <remarks>Issues a new certificate from a vault with the default vesting period</remarks>
        /// <param name="address">Vault contract address</param>
        /// <param name="request">Create vault certificate request.</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Cirrus transaction hash</returns>
        [HttpPost("{address}/certificates")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> CreateCertificate(string address,
                                                                  CreateVaultCertificateRequest request,
                                                                  CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new CreateWalletCreateVaultCertificateCommand(_context.Wallet, address,
                                                                                                     request.Holder, request.Amount), cancellationToken);

            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>Redeem Certificate Quote</summary>
        /// <remarks>Redeems all vested certificates in a vault, owned by the authorized wallet</remarks>
        /// <param name="address">Vault contract address</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Cirrus transaction hash</returns>
        [HttpPost("{address}/certificates/redeem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RedeemCertificates(string address, CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new CreateWalletRedeemVaultCertificateCommand(_context.Wallet, address), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }

        /// <summary>Revoke Certificate Quote</summary>
        /// <remarks>Revokes all non-vested certificates in a vault, held by a specified address</remarks>
        /// <param name="address">Vault contract address</param>
        /// <param name="request">Revoke certificate request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Cirrus transaction hash</returns>
        [HttpPost("{address}/certificates/revoke")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<string>> RevokeCertificates(string address,
                                                                RevokeVaultCertificatesRequest request,
                                                                CancellationToken cancellationToken)
        {
            var transactionHash = await _mediator.Send(new CreateWalletRevokeVaultCertificateCommand(_context.Wallet, address, request.Holder), cancellationToken);
            return Created(string.Format(_blockExplorerConfig.TransactionEndpoint, transactionHash), transactionHash);
        }
    }
}
