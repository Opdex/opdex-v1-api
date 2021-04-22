using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Abstractions
{
    public interface IIndexProcessManager
    {
        public Task ProcessAsync(CancellationToken cancellationToken);
    }
}