using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventDto : TransactionEventDto
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}
