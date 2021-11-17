using System.Numerics;

namespace Opdex.Platform.Common.Extensions
{
    public static class BigIntegerExtensions
    {
        public static BigInteger ToBigInteger(this string value)
        {
            var parsed = BigInteger.TryParse(value, out var parsedValue);
            return !parsed ? BigInteger.Zero : parsedValue;
        }
    }
}
