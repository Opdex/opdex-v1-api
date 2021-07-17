using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Governances
{
    public class NominationEventDtoAssembler : IModelAssembler<NominationLog, NominationEventDto>
    {
        public Task<NominationEventDto> Assemble(NominationLog log)
        {
            return Task.FromResult(new NominationEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.NominationEvent,
                MiningPool = log.MiningPool,
                StakingPool = log.StakingPool,
                Weight = log.Weight.InsertDecimal(TokenConstants.Opdex.Decimals)
            });
        }
    }
}
