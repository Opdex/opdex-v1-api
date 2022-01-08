using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers;

public class VaultCertificateDtoAssembler: IModelAssembler<VaultCertificate, VaultCertificateDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public VaultCertificateDtoAssembler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<VaultCertificateDto> Assemble(VaultCertificate certificate)
    {
        var certificateDto = _mapper.Map<VaultCertificateDto>(certificate);

        var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(certificate.ProposalId), CancellationToken.None);

        certificateDto.ProposalId = proposal.PublicId;

        return certificateDto;
    }
}
