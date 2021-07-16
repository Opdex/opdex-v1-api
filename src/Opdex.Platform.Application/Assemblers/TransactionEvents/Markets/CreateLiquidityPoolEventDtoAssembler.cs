using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventDtoAssembler :  IModelAssembler<CreateLiquidityPoolLog, CreateLiquidityPoolEventDto>
    {
        public Task<CreateLiquidityPoolEventDto> Assemble(CreateLiquidityPoolLog log)
        {
            return Task.FromResult(new CreateLiquidityPoolEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Pool = log.Pool,
                Token = log.Token
            });
        }
    }
}
