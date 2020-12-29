namespace Opdex.Indexer.Application.Abstractions.Models.Events
{
    public class MintEvent
    {
        public string Sender;
        public ulong AmountCrs;
        public ulong AmountToken; 
    }
}