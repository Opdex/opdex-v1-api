namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault
{
    public class ChangeVaultOwnerEventDto : TransactionEventDto
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
