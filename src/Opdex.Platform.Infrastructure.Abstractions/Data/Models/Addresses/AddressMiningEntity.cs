namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressMiningEntity
    {
        public long Id { get; set; }
        public long MiningPoolId { get; set; }
        public string Owner { get; set; }
        public string Balance { get; set; }
        public ulong CreatedBlock { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}