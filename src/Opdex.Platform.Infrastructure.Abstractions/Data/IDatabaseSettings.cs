using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data
{
    public interface IDatabaseSettings<out TConnection> where TConnection : IDbConnection
    {
        TConnection Create();
    }
}
