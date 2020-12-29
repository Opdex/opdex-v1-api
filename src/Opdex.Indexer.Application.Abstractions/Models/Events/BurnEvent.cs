namespace Opdex.Indexer.Application.Abstractions.Models.Events
{
    public class BurnEvent
    {
        public string Sender;
        public string To;
        public ulong AmountCrs;
        public ulong AmountToken;
    }
}