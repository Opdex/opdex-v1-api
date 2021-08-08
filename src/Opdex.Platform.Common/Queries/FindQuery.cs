using MediatR;

namespace Opdex.Platform.Common.Queries
{
    public abstract class FindQuery<T> : IRequest<T>
    {
        protected FindQuery(bool findOrThrow)
        {
            FindOrThrow = findOrThrow;
        }
        
        public bool FindOrThrow { get; }
    }
}