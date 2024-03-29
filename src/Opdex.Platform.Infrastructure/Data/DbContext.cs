using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Polly;
using Polly.Retry;
using System.Diagnostics;

namespace Opdex.Platform.Infrastructure.Data;

public class DbContext : IDbContext
{
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<MySqlException>(ex => ex.IsTransient)
        .Or<TimeoutException>()
        .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(x));

    private readonly ILogger<DbContext> _logger;
    private readonly IDatabaseSettings<MySqlConnection> _databaseSettings;

    public DbContext(DatabaseConfiguration databaseConfiguration, ILogger<DbContext> logger)
    {
        var configuration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var connectionStringBuilder = new MySqlConnectionStringBuilder(configuration.ConnectionString);
        _databaseSettings = new DatabaseSettings(connectionStringBuilder.ConnectionString);
    }

    public async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(DatabaseQuery query)
    {
        await using var connection = _databaseSettings.Create();
        return await Execute(query, async c => await connection.QueryAsync<TEntity>(c));
    }

    public async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(DatabaseQuery query,
                                                                                        Func<TFirst, TSecond, TReturn> map,
                                                                                        string splitOn)
    {
        await using var connection = _databaseSettings.Create();
        return await Execute(query, async c => await connection.QueryAsync(c, map, splitOn));
    }

    public async Task<TEntity> ExecuteFindAsync<TEntity>(DatabaseQuery query)
    {
        await using var connection = _databaseSettings.Create();
        return await Execute(query, async c => await connection.QuerySingleOrDefaultAsync<TEntity>(c));
    }

    public async Task<TEntity> ExecuteScalarAsync<TEntity>(DatabaseQuery query)
    {
        await using var connection = _databaseSettings.Create();
        return await Execute(query, async c => await connection.ExecuteScalarAsync<TEntity>(c));
    }

    public async Task<int> ExecuteCommandAsync(DatabaseQuery query)
    {
        await using var connection = _databaseSettings.Create();
        return await Execute(query, async c => await connection.ExecuteAsync(c));
    }

    private async Task<TR> Execute<TR>(DatabaseQuery query, Func<CommandDefinition, Task<TR>> action)
    {
        var command = new CommandDefinition(query.Sql, query.Parameters, commandType: query.Type, cancellationToken: query.Token);

        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await action.Invoke(command);
            stopwatch.Stop();

            using (_logger.BeginScope(new Dictionary<string, object>
                   {
                       { "Command", command.CommandText }
                   }))
            {
                _logger.LogDebug("Executed database query in {ExecutionTimeMs} ms", stopwatch.ElapsedMilliseconds);
            }

            return result;
        });
    }
}
