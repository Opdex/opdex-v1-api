using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Core.Infrastructure.Abstractions.Integrations
{
    public interface IApiClient
    {
        Task<TReturn> GetAsync<TReturn>(string uri, CancellationToken cancellationToken);
        Task<TReturn> PostAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken);
        Task<TReturn> PutAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken);
        Task<TReturn> PatchAsync<TReturn>(string uri, HttpContent httpContent, CancellationToken cancellationToken);
        Task<TReturn> DeleteAsync<TReturn>(string uri, CancellationToken cancellationToken);
    }
}