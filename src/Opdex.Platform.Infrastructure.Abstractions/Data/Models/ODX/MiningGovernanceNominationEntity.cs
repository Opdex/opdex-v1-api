namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX
{
    public class MiningGovernanceNominationEntity : AuditEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public long MiningPoolId { get; set; }
        public bool IsNominated { get; set; }
        public string Weight { get; set; }
    }
}