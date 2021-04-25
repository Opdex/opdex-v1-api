namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools
{
    public class MiningPoolEntity : AuditEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Address { get; set; }
        public string RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}