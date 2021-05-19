namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressStakingEntity : AuditEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Owner { get; set; }
        public string Weight { get; set; }
    }
}