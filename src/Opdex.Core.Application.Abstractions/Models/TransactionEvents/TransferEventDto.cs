namespace Opdex.Core.Application.Abstractions.Models.TransactionEvents
{
    public class TransferEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}