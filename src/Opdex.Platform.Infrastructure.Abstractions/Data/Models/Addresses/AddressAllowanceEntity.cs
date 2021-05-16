namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressAllowanceEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Allowance { get; set; }
        public ulong CreatedBlock { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}