namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers
{
    public class ChangeDeployerOwnerEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public override TransactionEventType EventType => TransactionEventType.ChangeDeployerOwnerEvent;
    }
}
