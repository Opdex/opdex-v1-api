using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens
{
    public class ApprovalEventDtoAssembler : IModelAssembler<ApprovalLog, ApprovalEventDto>
    {
        private readonly IMediator _mediator;

        public ApprovalEventDtoAssembler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ApprovalEventDto> Assemble(ApprovalLog log)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(log.Contract));

            return new ApprovalEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                Owner = log.Owner,
                Spender = log.Spender,
                Amount = log.Amount.ToDecimal(token.Decimals)
            };
        }
    }
}
