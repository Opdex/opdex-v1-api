namespace Opdex.Platform.WebApi.Models.Responses.TransactionEvents
{
    public class PairCreatedEventResponseModel : TransactionEventResponseModelBase
    {
        public string Token { get; set; }
        public string Pair { get; set; }
    }
}