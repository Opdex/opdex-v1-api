using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.ProposalCertificates;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultProposalDtoAssembler : IModelAssembler<VaultProposal, VaultProposalDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IModelAssembler<VaultCertificate, VaultCertificateDto> _certificateAssembler;

    public VaultProposalDtoAssembler(IMapper mapper, IMediator mediator, IModelAssembler<VaultCertificate, VaultCertificateDto> certificateAssembler)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _certificateAssembler = certificateAssembler ?? throw new ArgumentNullException(nameof(certificateAssembler));
    }

    public async Task<VaultProposalDto> Assemble(VaultProposal proposal)
    {
        var dto = _mapper.Map<VaultProposalDto>(proposal);

        var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(proposal.VaultGovernanceId));
        var token = await _mediator.Send(new RetrieveTokenByIdQuery(vault.TokenId));

        dto.Vault = vault.Address;
        dto.Token = token.Address;
        dto.Amount = proposal.Amount.ToDecimal(token.Decimals);
        dto.PledgeAmount = proposal.PledgeAmount.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.YesAmount = proposal.YesAmount.ToDecimal(TokenConstants.Cirrus.Decimals);
        dto.NoAmount = proposal.NoAmount.ToDecimal(TokenConstants.Cirrus.Decimals);

        var isApprovedCreation = dto.Type == VaultProposalType.Create && dto.Approved;
        var isRevocation = dto.Type == VaultProposalType.Revoke;

        if (!isApprovedCreation && !isRevocation) return dto;

        var proposalCertificate = await _mediator.Send(new RetrieveVaultProposalCertificateByProposalIdQuery(proposal.Id));
        var certificate = await _mediator.Send(new RetrieveVaultCertificateByIdQuery(proposalCertificate.CertificateId));
        dto.Certificate = await _certificateAssembler.Assemble(certificate);

        return dto;
    }
}
