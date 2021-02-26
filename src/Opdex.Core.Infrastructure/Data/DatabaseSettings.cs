using Microsoft.Data.Sqlite;
using MySqlConnector;
using Opdex.Core.Infrastructure.Abstractions.Data;

namespace Opdex.Core.Infrastructure.Data
{
    public sealed class DatabaseSettings: IDatabaseSettings<MySqlConnection>
    {
        private readonly string _connectionString;

        public DatabaseSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection Create()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
