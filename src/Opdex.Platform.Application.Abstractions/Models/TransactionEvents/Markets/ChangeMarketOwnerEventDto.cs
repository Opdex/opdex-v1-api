namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets
{
    public class ChangeMarketOwnerEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
