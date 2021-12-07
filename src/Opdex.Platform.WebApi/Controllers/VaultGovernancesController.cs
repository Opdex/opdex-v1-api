using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Responses.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("vault-governances")]
public class VaultGovernancesController : ControllerBase
{
    private readonly IApplicationContext _context;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultGovernancesController(IApplicationContext context, IMapper mapper, IMediator mediator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>Get Vault</summary>
    /// <remarks>Retrieves vault details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault details.</returns>
    /// <response code="200">Vault details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Vault not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpGet("{address}")]
    [ProducesResponseType(typeof(VaultGovernanceResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VaultGovernanceResponseModel>> GetVault([FromRoute] Address address, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultGovernanceByAddressQuery(address), cancellationToken);
        var response = _mapper.Map<VaultGovernanceResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Propose Create Certificate</summary>
    /// <remarks>Quotes making a vault proposal to create a certificate.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Vault not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/create-certificate")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeCreateCertificate([FromRoute] Address address,
                                                                                                 [FromBody] CreateCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalCreateCertificateQuoteCommand(address, request.Owner, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Revoke Certificate</summary>
    /// <remarks>Quotes making a vault proposal to revoke a certificate.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Vault not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/revoke-certificate")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeRevokeCertificate([FromRoute] Address address,
                                                                                                 [FromBody] RevokeCertificateVaultProposalQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalRevokeCertificateQuoteCommand(address, request.Owner, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Pledge</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum pledge amount.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Vault not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/minimum-pledge")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumPledge([FromRoute] Address address,
                                                                                             [FromBody] MinimumPledgeVaultProposalQuoteRequest request,
                                                                                             CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumPledgeQuoteCommand(address, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Propose Change Minimum Vote</summary>
    /// <remarks>Quotes making a vault proposal to change the minimum vote amount.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="request">Vault proposal details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal quote.</returns>
    /// <response code="200">Vault proposal quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Vault not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/minimum-vote")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteProposeMinimumVote([FromRoute] Address address, [FromBody] MinimumVoteVaultProposalQuoteRequest request,
                                                                                           CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalMinimumVoteQuoteCommand(address, request.Amount, request.Description, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal</summary>
    /// <remarks>Retrieves vault proposal details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal details.</returns>
    /// <response code="200">Vault proposal details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpGet("{address}/proposals/{proposalId}")]
    [ProducesResponseType(typeof(VaultProposalResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VaultProposalResponseModel>> GetProposal([FromRoute] Address address, [FromRoute] ulong proposalId, CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalByVaultAddressAndPublicIdQuery(address, proposalId), cancellationToken);
        var response = _mapper.Map<VaultProposalResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Vault Proposal Pledge</summary>
    /// <remarks>Quotes a vault proposal pledge.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge quote.</returns>
    /// <response code="200">Vault proposal pledge quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/{proposalId}/pledges")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuotePledge([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                               [FromBody] VaultProposalPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalPledgeQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Pledge</summary>
    /// <remarks>Quotes withdrawing a vault proposal pledge.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw pledge request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw pledge quote.</returns>
    /// <response code="200">Vault proposal withdraw pledge quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/{proposalId}/pledges/withdraw")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawPledge([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                                     [FromBody] VaultProposalWithdrawPledgeQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalPledgeQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Pledge</summary>
    /// <remarks>Retrieves vault proposal pledge details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="pledger">Address of the pledger.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge details.</returns>
    /// <response code="200">Vault proposal pledge details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault, proposal or pledger not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpGet("{address}/proposals/{proposalId}/pledges/{pledger}")]
    [ProducesResponseType(typeof(VaultProposalPledgeResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VaultProposalPledgeResponseModel>> GetPledge([FromRoute] Address address, [FromRoute] ulong proposalId, [FromRoute] Address pledger,
                                                                                CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalPledgeByVaultAddressPublicIdAndPledgerQuery(address, proposalId, pledger), cancellationToken);
        var response = _mapper.Map<VaultProposalPledgeResponseModel>(dto);
        return Ok(response);
    }

    /// <summary>Quote Vault Proposal Vote</summary>
    /// <remarks>Quotes a vault proposal vote.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote quote.</returns>
    /// <response code="200">Vault proposal vote quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/{proposalId}/votes")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteVote([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                             [FromBody] VaultProposalVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateVaultProposalVoteQuoteCommand(address, proposalId, request.Amount, request.InFavor, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Quote Withdraw Vault Proposal Vote</summary>
    /// <remarks>Quotes withdrawing a vault proposal vote.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="request">Vault proposal withdraw vote request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal withdraw vote quote.</returns>
    /// <response code="200">Vault proposal withdraw vote quote created.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault or proposal not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpPost("{address}/proposals/{proposalId}/votes/withdraw")]
    [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionQuoteResponseModel>> QuoteWithdrawVote([FromRoute] Address address, [FromRoute] ulong proposalId,
                                                                                     [FromBody] VaultProposalWithdrawVoteQuoteRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateWithdrawVaultProposalVoteQuoteCommand(address, proposalId, request.Amount, _context.Wallet), cancellationToken);
        var quote = _mapper.Map<TransactionQuoteResponseModel>(response);
        return Ok(quote);
    }

    /// <summary>Get Vault Proposal Vote</summary>
    /// <remarks>Retrieves vault proposal vote details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="voter">Address of the voter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote details.</returns>
    /// <response code="200">Vault proposal vote details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault, proposal or voter not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpGet("{address}/proposals/{proposalId}/votes/{voter}")]
    [ProducesResponseType(typeof(VaultProposalVoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VaultProposalVoteResponseModel>> GetVote([FromRoute] Address address, [FromRoute] ulong proposalId, [FromRoute] Address voter,
                                                                            CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(address, proposalId, voter), cancellationToken);
        var response = _mapper.Map<VaultProposalVoteResponseModel>(dto);
        return Ok(response);
    }
}
