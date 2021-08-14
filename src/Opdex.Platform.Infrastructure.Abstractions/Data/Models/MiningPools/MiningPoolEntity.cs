namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools
{
    public class MiningPoolEntity : AuditEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Address { get; set; }
        public string RewardPerBlock { get; set; }
        public string RewardPerLpt { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}