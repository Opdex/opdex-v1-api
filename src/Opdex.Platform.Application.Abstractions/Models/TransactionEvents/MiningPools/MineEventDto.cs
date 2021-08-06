using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools
{
    public abstract class MineEventDto : TransactionEventDto
    {
        public string Miner { get; set; }
        public string Amount { get; set; }
        public string TotalSupply { get; set; }
        public string MinerBalance { get; set; }
    }
}
