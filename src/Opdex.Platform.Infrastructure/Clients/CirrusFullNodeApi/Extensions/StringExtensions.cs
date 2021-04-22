using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Extensions
{
    public static class StringExtensions
    {
        public static string HexToString(this string value)
        {
            return Encoding.UTF8.GetString(value.HexToByteArray());
        }
    }
}