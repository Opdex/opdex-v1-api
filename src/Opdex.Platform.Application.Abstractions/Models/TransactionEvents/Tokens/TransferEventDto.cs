using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class TransferLogDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}
