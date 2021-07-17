namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class TransferEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}
