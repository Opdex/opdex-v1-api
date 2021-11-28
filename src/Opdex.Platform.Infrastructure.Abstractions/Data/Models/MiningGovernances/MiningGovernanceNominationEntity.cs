using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances
{
    public class MiningGovernanceNominationEntity : AuditEntity
    {
        public ulong Id { get; set; }
        public ulong MiningGovernanceId { get; set; }
        public ulong LiquidityPoolId { get; set; }
        public ulong MiningPoolId { get; set; }
        public bool IsNominated { get; set; }
        public UInt256 Weight { get; set; }
    }
}
