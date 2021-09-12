using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models
{
    public interface IApplicationContext
    {
        Address Market { get; }
        Address Wallet { get; }
    }
}
