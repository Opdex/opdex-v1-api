using System.Collections.Generic;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Abstractions.Data
{
    public interface IDbContext
    {
        Task<TEntity> ExecuteFindAsync<TEntity>(DatabaseQuery query);
        Task<TScalar> ExecuteScalarAsync<TScalar>(DatabaseQuery query);
        Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(DatabaseQuery query);
        Task<int> ExecuteCommandAsync(DatabaseQuery query);
    }
}