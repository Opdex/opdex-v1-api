using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Opdex.Platform.Infrastructure.Abstractions.Data;

namespace Opdex.Platform.Infrastructure.Data
{
    // Todo Implement Dispose Methods
    // Ref: https://stackoverflow.com/questions/538060/proper-use-of-the-idisposable-interface
    public class DbContext : IDbContext
    {
        private readonly ILogger<DbContext> _logger;
        private readonly IDatabaseSettings<MySqlConnection> _databaseSettings;

        public DbContext(DatabaseConfiguration databaseConfiguration, ILogger<DbContext> logger)
        {
            var configuration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
            var connectionString = new MySqlConnectionStringBuilder(configuration.ConnectionString);

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseSettings = new DatabaseSettings(connectionString.ConnectionString);
        }

        public async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(DatabaseQuery query)
        {
            try
            {
                var command = BuildCommandDefinition(query);
                await using var connection = _databaseSettings.Create();
                return await connection.QueryAsync<TEntity>(command);
            }
            catch (Exception ex)
            {
                const string error = "Failure to execute query.";
                _logger.LogError(ex, error);
                throw;
            }
        }

        public async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TFirst, TSecond, TReturn>(DatabaseQuery query,
                                                                                            Func<TFirst, TSecond, TReturn> map,
                                                                                            string splitOn)
        {
            try
            {
                var command = BuildCommandDefinition(query);
                await using var connection = _databaseSettings.Create();
                return await connection.QueryAsync<TFirst, TSecond, TReturn>(command, map, splitOn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure to execute query.");
                throw;
            }
        }

        public async Task<TEntity> ExecuteFindAsync<TEntity>(DatabaseQuery query)
        {
            try
            {
                var command = BuildCommandDefinition(query);
                await using var connection = _databaseSettings.Create();
                return await connection.QuerySingleOrDefaultAsync<TEntity>(command);
            }
            catch (Exception ex)
            {
                 const string error = "Failure to execute find.";
                _logger.LogError(ex, error);
                throw;
            }
        }

        public async Task<TEntity> ExecuteScalarAsync<TEntity>(DatabaseQuery query)
        {
            try
            {
                var command = BuildCommandDefinition(query);
                await using var connection = _databaseSettings.Create();
                return await connection.ExecuteScalarAsync<TEntity>(command);
            }
            catch (Exception ex)
            {
                 const string error = "Failure to execute scalar.";
                _logger.LogError(ex, error);
                throw;
            }
        }

        public async Task<int> ExecuteCommandAsync(DatabaseQuery query)
        {
            try
            {
                var command = BuildCommandDefinition(query);
                await using var connection = _databaseSettings.Create();
                return await connection.ExecuteAsync(command);
            }
            catch (Exception ex)
            {
                 const string error = "Failure to execute command.";
                _logger.LogError(ex, error);
                throw;
            }
        }

        private static CommandDefinition BuildCommandDefinition(DatabaseQuery query)
        {
            return new CommandDefinition(query.Sql, query.Parameters, commandType: query.Type, cancellationToken: query.Token);
        }
    }
}
