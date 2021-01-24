using System.Data;

namespace Opdex.Core.Infrastructure.Abstractions.Data
{
    public interface IDatabaseSettings<out TConnection> where TConnection : IDbConnection
    {
        TConnection Create();
    }
}
