namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class SyncEventResponseModel : TransactionEventResponseModelBase
    {
        public ulong ReserveCrs { get; set; }
        public string ReserveSrc { get; set; }
    }
}