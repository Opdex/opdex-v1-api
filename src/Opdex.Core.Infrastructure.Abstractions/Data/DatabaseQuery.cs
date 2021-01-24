using System.Data;
using System.Threading;

namespace Opdex.Core.Infrastructure.Abstractions.Data
{
    public readonly struct DatabaseQuery
    {
        public string Sql { get; }
        public object Parameters { get; }
        public CommandType Type { get; }
        public CancellationToken Token { get; }

        private DatabaseQuery(string sql, object parameters = null, CancellationToken token = default,
            CommandType type = CommandType.Text)
        {
            Sql = sql;
            Parameters = parameters;
            Token = token;
            Type = type;
        }

        public static DatabaseQuery Create(string sql, object parameters = null, CancellationToken token = default,
            CommandType type = CommandType.Text)
        {
            return new DatabaseQuery(sql, parameters, token, type);
        }
    }
}