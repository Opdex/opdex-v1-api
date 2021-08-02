namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents
{
    public abstract class OwnershipEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
