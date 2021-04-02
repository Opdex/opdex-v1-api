namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class SmartContractCallResponseDto
    {
        public ulong Fee { get; set; }
        public string Hex { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; }
    }
}