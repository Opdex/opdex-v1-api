using Microsoft.Data.Sqlite;
using Opdex.Core.Infrastructure.Abstractions.Data;

namespace Opdex.Core.Infrastructure.Data
{
    public sealed class DatabaseSettings: IDatabaseSettings<SqliteConnection>
    {
        private readonly string _connectionString;

        public DatabaseSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqliteConnection Create()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
