using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Common.Extensions;

public static class NetworkTypeExtensions
{
    public static string NetworkTokenPrefix(this NetworkType network)
    {
        if (network == NetworkType.MAINNET) return string.Empty;

        return network == NetworkType.TESTNET ? "T" : "D";
    }
}