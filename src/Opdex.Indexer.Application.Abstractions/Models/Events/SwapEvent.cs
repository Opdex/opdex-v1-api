namespace Opdex.Indexer.Application.Abstractions.Models.Events
{
    public class SwapEvent
    {
        public string Sender;
        public string To;
        public ulong AmountCrsIn;
        public ulong AmountTokenIn;
        public ulong AmountCrsOut;
        public ulong AmountTokenOut;
    }
}