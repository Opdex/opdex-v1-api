namespace Opdex.Platform.WebApi.Models
{
    public class ReIndexRequest
    {
        public ulong From { get; set; }
        public ulong To { get; set; }
        
        // Todo: Eventually can add other options to target pools, tokens, transactions<type>
    }
}