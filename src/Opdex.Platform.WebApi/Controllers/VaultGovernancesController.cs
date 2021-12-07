using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.EntryQueries.VaultGovernances.Votes;
using Opdex.Platform.Common.Models;
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
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultGovernancesController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get Vault
    /// </summary>
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

    /// <summary>
    /// Get Vault Proposal
    /// </summary>
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

    /// <summary>
    /// Get Vault Proposal Pledge
    /// </summary>
    /// <remarks>Retrieves vault proposal pledge details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="pledger">Address of the pledger.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal pledge details.</returns>
    /// <response code="200">Vault proposal pledge details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault, proposal or pledge not found.</response>
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

    /// <summary>
    /// Get Vault Proposal Vote
    /// </summary>
    /// <remarks>Retrieves vault proposal vote details.</remarks>
    /// <param name="address">Address of the vault.</param>
    /// <param name="proposalId">Id of the proposal in the vault.</param>
    /// <param name="voter">Address of the voter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vault proposal vote details.</returns>
    /// <response code="200">Vault proposal vote details found.</response>
    /// <response code="400">The request is not valid.</response>
    /// <response code="404">Either vault, proposal or vote not found.</response>
    /// <response code="429">Too many requests.</response>
    [HttpGet("{address}/proposals/{proposalId}/votes/{voter}")]
    [ProducesResponseType(typeof(VaultProposalVoteResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VaultProposalVoteResponseModel>> GetVote([FromRoute] Address address, [FromRoute] ulong proposalId, [FromRoute] Address voter,
                                                                            CancellationToken cancellationToken)
    {
        var dto = await _mediator.Send(new GetVaultProposalVoteByVaultAddressPublicIdAndVoterQuery(address, proposalId, voter), cancellationToken);
        var response = _mapper.Map<VaultProposalVoteResponseModel>(dto);
        return Ok(response);
    }
}
