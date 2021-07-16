namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers
{
    public class ChangeDeployerOwnerLogDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
