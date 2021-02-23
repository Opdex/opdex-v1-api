using System.Text;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Opdex.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        
        public static string HexToString(this string value)
        {
            return Encoding.UTF8.GetString(value.HexToByteArray());
        }
    }
}