namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class PoolCreatedEventResponseModel : TransactionEventResponseModelBase
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}