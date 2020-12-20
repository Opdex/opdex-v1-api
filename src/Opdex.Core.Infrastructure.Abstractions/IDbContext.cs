using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Opdex.Core.Infrastructure.Abstractions
{
    public interface IDbContext : IDisposable
        {
            Task<IDbConnection> OpenAsync(string connectionString);
            Task<IDbTransaction> BeginTransactionAsync();
            Task<IDbTransaction> BeginTransactionAsync(bool isReadOnly);
            Task CommitAsync();
            Task RollbackAsync();

            IDbConnection Connection { get; }

            Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(DatabaseQuery query);
            Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(DatabaseQuery query,
                Func<TFirst, TSecond, TReturn> map, string splitOn);
            Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TThird, TReturn>(DatabaseQuery query,
                Func<TFirst, TSecond, TThird, TReturn> map, string splitOn);

            Task<TEntity> ExecuteFindAsync<TEntity>(DatabaseQuery query);

            Task<TReturn> ExecuteFindAsync<TFirst, TSecond, TReturn>(DatabaseQuery query,
                Func<TFirst, TSecond, TReturn> map, string splitOn);
            Task<TReturn> ExecuteFindAsync<TFirst, TSecond, TThird, TReturn>(DatabaseQuery query,
                Func<TFirst, TSecond, TThird, TReturn> map, string splitOn);

            Task<int> ExecuteAsync(DatabaseQuery query);

            Task<TScalar> ExecuteScalarAsync<TScalar>(DatabaseQuery query);
    }
}