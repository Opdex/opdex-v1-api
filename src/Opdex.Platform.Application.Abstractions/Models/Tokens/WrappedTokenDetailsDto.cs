using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens;

public class WrappedTokenDetailsDto
{
    public ExternalChainType Chain { get; set; }
    public string Address { get; set; }
}
