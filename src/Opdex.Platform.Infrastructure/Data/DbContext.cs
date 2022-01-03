using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Opdex.Platform.Infrastructure.Abstractions.Data;

namespace Opdex.Platform.Infrastructure.Data;

// Todo Implement Dispose Methods
// Ref: https://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface
public class DbContext : IDbContext
{
    private readonly IDatabaseSettings<MySqlConnection> _databaseSettings;

    public DbContext(DatabaseConfiguration databaseConfiguration)
    {
        var configuration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        var connectionString = new MySqlConnectionStringBuilder(configuration.ConnectionString);

        _databaseSettings = new DatabaseSettings(connectionString.ConnectionString);
    }

    public async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(DatabaseQuery query)
    {
        var command = BuildCommandDefinition(query);
        await using var connection = _databaseSettings.Create();
        return await connection.QueryAsync<TEntity>(command);
    }

    public async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(DatabaseQuery query,
                                                                                        Func<TFirst, TSecond, TReturn> map,
                                                                                        string splitOn = "Id")
    {
        var command = BuildCommandDefinition(query);
        await using var connection = _databaseSettings.Create();
        return await connection.QueryAsync(command, map, splitOn);
    }

    public async Task<TEntity> ExecuteFindAsync<TEntity>(DatabaseQuery query)
    {
        var command = BuildCommandDefinition(query);
        await using var connection = _databaseSettings.Create();
        return await connection.QuerySingleOrDefaultAsync<TEntity>(command);
    }

    public async Task<TEntity> ExecuteScalarAsync<TEntity>(DatabaseQuery query)
    {
        var command = BuildCommandDefinition(query);
        await using var connection = _databaseSettings.Create();
        return await connection.ExecuteScalarAsync<TEntity>(command);
    }

    public async Task<int> ExecuteCommandAsync(DatabaseQuery query)
    {
        var command = BuildCommandDefinition(query);
        await using var connection = _databaseSettings.Create();
        return await connection.ExecuteAsync(command);
    }

    private static CommandDefinition BuildCommandDefinition(DatabaseQuery query)
    {
        return new CommandDefinition(query.Sql, query.Parameters, commandType: query.Type, cancellationToken: query.Token);
    }
}
