using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens;

public class TransferEventDtoAssembler : IModelAssembler<TransferLog, TransferEventDto>
{
    private readonly IMediator _mediator;

    public TransferEventDtoAssembler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TransferEventDto> Assemble(TransferLog log)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(log.Contract));

        return new TransferEventDto
        {
            Id = log.Id,
            TransactionId = log.TransactionId,
            SortOrder = log.SortOrder,
            Contract = log.Contract,
            From = log.From,
            To = log.To,
            Amount = log.Amount.ToDecimal(token.Decimals)
        };
    }
}