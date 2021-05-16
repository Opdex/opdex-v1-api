namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressBalanceEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public long LiquidityPoolId { get; set; }
        public string Owner { get; set; }
        public string Balance { get; set; }
        public ulong CreatedBlock { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}