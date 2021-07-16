using Opdex.Platform.Application.Abstractions.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class ChangeMarketOwnerLogDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
