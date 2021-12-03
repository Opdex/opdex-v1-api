using MySqlConnector;
using Opdex.Platform.Infrastructure.Abstractions.Data;

namespace Opdex.Platform.Infrastructure.Data;

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