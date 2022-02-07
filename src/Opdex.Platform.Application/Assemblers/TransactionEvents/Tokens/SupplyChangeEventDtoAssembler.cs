using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens;

public class SupplyChangeEventDtoAssembler : IModelAssembler<SupplyChangeLog, SupplyChangeEventDto>
{
    private readonly IMediator _mediator;

    public SupplyChangeEventDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<SupplyChangeEventDto> Assemble(SupplyChangeLog log)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(log.Contract), CancellationToken.None);

        return new SupplyChangeEventDto()
        {
            Id = log.Id,
            TransactionId = log.TransactionId,
            SortOrder = log.SortOrder,
            Contract = log.Contract,
            PreviousTotalSupply = log.PreviousSupply.ToDecimal(token.Decimals),
            UpdatedTotalSupply = log.TotalSupply.ToDecimal(token.Decimals)
        };
    }
}
