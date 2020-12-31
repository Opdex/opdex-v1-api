using MediatR;

namespace Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events
{
    public class RetrieveCirrusEventBaseQuery<T> : IRequest<T>
    {
        public RetrieveCirrusEventBaseQuery(ulong from, ulong to, string contract)
        {
            From = from;
            To = to;
            Contract = contract;
        }
        
        public ulong From { get; }
        public ulong To { get; }
        public string Contract { get; }
    }
}