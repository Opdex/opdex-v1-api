namespace Opdex.Indexer.Application.Abstractions.Models.Events
{
    public class PairCreated
    {
        public string Token { get; set; }
        public string Pair { get; set; }
    }
}