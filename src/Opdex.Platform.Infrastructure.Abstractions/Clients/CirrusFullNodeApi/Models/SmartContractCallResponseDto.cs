using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class SmartContractCallResponseDto
    {
        public ulong Fee { get; set; }
        public string Hex { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public Sha256 TransactionId { get; set; }
    }
}
