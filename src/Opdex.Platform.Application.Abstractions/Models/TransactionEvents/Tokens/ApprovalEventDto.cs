using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class ApprovalLogDto : TransactionEventDto
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}
