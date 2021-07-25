using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries
{
    public abstract class SelectFilterQuery<T> : IRequest<T>
    {
        protected SelectFilterQuery()
        {

        }
    }
}
