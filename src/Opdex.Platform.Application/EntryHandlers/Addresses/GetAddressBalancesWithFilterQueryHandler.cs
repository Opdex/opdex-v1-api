using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetAddressBalancesWithFilterQueryHandler : EntryFilterQueryHandler<GetAddressBalancesWithFilterQuery, AddressBalancesDto>
    {
        private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _assembler;

        private const string PagingBase = "wallet:{0};direction:{1};limit:{2};includeLpTokens:{3};includeZeroBalnces:{4};<tokens>";

        public GetAddressBalancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> assembler)
            : base(mediator)
        {
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public override async Task<AddressBalancesDto> Handle(GetAddressBalancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new RetrieveAddressBalancesWithFilterQuery(request.Wallet, request.Tokens, request.IncludeLpTokens,
                                                                                           request.IncludeZeroBalances, request.Direction, request.Limit,
                                                                                           request.Next, request.Previous), cancellationToken);

            // Assemble Dtos
            var balanceDtos = await Task.WhenAll(balances.Select(balance => _assembler.Assemble(balance)));

            // Sort
            var sortedDtos = request.Direction == SortDirectionType.ASC
                ? balanceDtos.OrderBy(t => t.Id).ToList()
                : balanceDtos.OrderByDescending(t => t.Id).ToList();

            // Build the default cursor without next or previous
            var baseCursor = string.Format(PagingBase, request.Wallet, request.Direction, request.Limit,
                                           request.IncludeLpTokens, request.IncludeZeroBalances)
                .Replace("<tokens>", BuildCursorFromList("eventTypes", request.Tokens.Select(e => e)));

            // The count can change if we remove the + 1 record, we want the original
            var sortedDtosCount = sortedDtos.Count;
            var limitPlusOne = request.Limit + 1;

            // Remove the + 1 value if necessary
            var removeAtIndex = RemoveAtIndex(request.PagingBackward, sortedDtosCount, limitPlusOne);
            if (removeAtIndex.HasValue)
            {
                sortedDtos.RemoveAt(removeAtIndex.Value);
            }

            // Gather first and last values of the response set to build cursors after the + 1 has been removed.
            var firstCursorValue = sortedDtos.FirstOrDefault()?.Id.ToString();
            var lastCursorValue = sortedDtos.LastOrDefault()?.Id.ToString();

            // Build the cursor DTO
            var cursor = BuildCursorDto(request.PagingBackward, request.PagingForward, sortedDtosCount,
                                        limitPlusOne, baseCursor, firstCursorValue, lastCursorValue);

            return new AddressBalancesDto { Balances = sortedDtos, Cursor = cursor };
        }
    }
}
