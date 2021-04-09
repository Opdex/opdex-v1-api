namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class ReservesLogResponseModel : TransactionLogResponseModelBase
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}