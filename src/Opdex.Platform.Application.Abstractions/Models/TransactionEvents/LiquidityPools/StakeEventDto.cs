using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools
{
    public class StakeEventDto : TransactionEventDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string TotalStaked { get; set; }
        public byte EventType { get; set; }
    }
}
