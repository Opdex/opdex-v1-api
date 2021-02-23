namespace Opdex.Core.Domain.Models
{
    public class Transaction
    {
        public Transaction()
        {
            
        }
        
        public long Id { get; set; }
        public string TxHash { get; set; }
        public int GasUsed { get; set; }
        public string From { get; set; }
        public ulong Block { get; set; }
        public string To { get; set; }
    }
}