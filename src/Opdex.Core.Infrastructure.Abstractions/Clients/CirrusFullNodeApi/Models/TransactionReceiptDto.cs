namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class TransactionReceiptDto
    {
        public string TransactionHash { get; set; }
        public string BlockHash { get; set; }
        public string PostState { get; set; }
        public ulong GasUsed { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string NewContractAddress { get; set; }
        public bool Success { get; set; }
        public string ReturnValue { get; set; }
        public string Bloom { get; set; }
        public string Error { get; set; }
        public TransactionLogDto[] Logs { get; set; }
    }
}