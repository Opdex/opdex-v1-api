using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class GetVaultCertificatesWithFilterQueryHandler : IRequestHandler<GetVaultCertificatesWithFilterQuery, CertificatesDto>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetVaultCertificatesWithFilterQueryHandler(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<CertificatesDto> Handle(GetVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
            var certificates = await _mediator.Send(new RetrieveVaultCertificatesWithFilterQuery(vault.Id, request.Cursor), cancellationToken);

            var certificatesResults = certificates.ToList();

            // The count can change if we remove the + 1 record, we want the original
            var resultsCount = certificatesResults.Count;
            var limitPlusOne = request.Cursor.Limit + 1;

            // Remove the + 1 value if necessary
            var removeAtIndex = RemoveAtIndex(request.Cursor.PagingDirection, resultsCount, limitPlusOne);
            if (removeAtIndex.HasValue)
            {
                certificatesResults.RemoveAt(removeAtIndex.Value);
            }

            // Gather first and last values of the response set to build cursors after the + 1 has been removed.
            var firstCursorValue = certificatesResults.FirstOrDefault()?.Id ?? 0;
            var lastCursorValue = certificatesResults.LastOrDefault()?.Id ?? 0;

            // Build the cursor DTO
            var cursorDto = resultsCount > 0 ? BuildCursorDto(request.Cursor, resultsCount, limitPlusOne, firstCursorValue, lastCursorValue) : new CursorDto();

            return new CertificatesDto { Certificates = _mapper.Map<IEnumerable<CertificateDto>>(certificatesResults), Cursor = cursorDto };
        }

        private int? RemoveAtIndex(PagingDirection pagingDirection, int count, uint limitPlusOne)
        {
            if (count < limitPlusOne) return null;

            return pagingDirection == PagingDirection.Backward ? 0 : count - 1;
        }

        private CursorDto BuildCursorDto(VaultCertificatesCursor cursor, int recordsFound, uint limitPlusOne, long firstRecordCursor, long lastRecordCursor)
        {
            var dto = new CursorDto();

            if (recordsFound == limitPlusOne)
            {
                dto.Next = cursor.Turn(lastRecordCursor).ToString().Base64Encode();

                if (cursor.PagingDirection == PagingDirection.Backward)
                {
                    dto.Previous = cursor.Turn(firstRecordCursor).ToString().Base64Encode();
                }
                else if (cursor.PagingDirection == PagingDirection.Forward && !cursor.IsNewRequest)
                {
                    dto.Previous = cursor.Turn(firstRecordCursor).ToString().Base64Encode();
                }
            }
            else if (!cursor.IsNewRequest)
            {
                if (cursor.PagingDirection == PagingDirection.Backward)
                {
                    dto.Next = cursor.Turn(lastRecordCursor).ToString().Base64Encode();
                }

                if (cursor.PagingDirection == PagingDirection.Forward)
                {
                    dto.Previous = cursor.Turn(firstRecordCursor).ToString().Base64Encode();
                }
            }

            return dto;
        }
    }
}
