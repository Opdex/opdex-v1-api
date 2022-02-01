namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

public class TokenChainEntity
{
    public ulong Id { get; set; }
    public ulong TokenId { get; set; }
    public ushort NativeChainTypeId { get; set; }
    public string NativeAddress { get; set; }
}
