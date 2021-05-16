namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressStakingEntity
    {
        public long Id { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Owner { get; set; }
        public string Weight { get; set; }
        public ulong CreatedBlock { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}