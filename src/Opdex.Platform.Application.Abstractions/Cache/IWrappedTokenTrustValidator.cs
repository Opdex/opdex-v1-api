using Opdex.Platform.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Abstractions.Cache;

public interface IWrappedTokenTrustValidator
{
    ValueTask<bool> Validate(Address token, CancellationToken cancellationToken = default);
}