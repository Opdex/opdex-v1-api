namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Markets
{
    public class ChangeMarketPermissionLogDto
    {
        public string Address { get; set;  }
        public byte Permission { get; set; }
        public bool IsAuthorized { get; set; }
    }
}