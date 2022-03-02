using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class InterfluxMappingDto
{
    public NativeChainType NativeNetwork { get; set; }
    public string NativeAddress { get; set; }
    public Address Src20Address { get; set; }
    public string TokenName { get; set; }
}
