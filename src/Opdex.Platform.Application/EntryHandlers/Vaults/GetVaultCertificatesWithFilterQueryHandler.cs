using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class GetVaultCertificatesWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultCertificatesWithFilterQuery, CertificatesDto>
    {
        private const string PagingBase = "holder:{0};direction:{1};limit:{2};";
        private readonly IMapper _mapper;

        public GetVaultCertificatesWithFilterQueryHandler(IMapper mapper, IMediator mediator) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<CertificatesDto> Handle(GetVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
            var certificates = await _mediator.Send(new RetrieveVaultCertificatesWithFilterQuery(vault.Id, request.Holder, request.Direction,
                                                                                                 request.Limit, request.Next, request.Previous), cancellationToken);

            var certificatesResults = certificates.ToList();

            // Build the default cursor without next or previous
            var baseCursor = string.Format(PagingBase, request.Holder, request.Direction, request.Limit);

            // The count can change if we remove the + 1 record, we want the original
            var resultsCount = certificatesResults.Count;
            var limitPlusOne = request.Limit + 1;

            // Remove the + 1 value if necessary
            var removeAtIndex = RemoveAtIndex(request.PagingBackward, resultsCount, limitPlusOne);
            if (removeAtIndex.HasValue)
            {
                certificatesResults.RemoveAt(removeAtIndex.Value);
            }

            // Gather first and last values of the response set to build cursors after the + 1 has been removed.
            var firstCursorValue = certificatesResults.FirstOrDefault()?.Id.ToString();
            var lastCursorValue = certificatesResults.LastOrDefault()?.Id.ToString();

            // Build the cursor DTO
            var cursor = BuildCursorDto(request.PagingBackward, request.PagingForward, resultsCount,
                                        limitPlusOne, baseCursor, firstCursorValue, lastCursorValue);

            return new CertificatesDto { Certificates = _mapper.Map<IEnumerable<CertificateDto>>(certificatesResults), Cursor = cursor };
        }
    }
}
