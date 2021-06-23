using System.Numerics;

namespace Opdex.Platform.Common.Extensions
{
    public static class BigIntegerExtensions
    {
        public static BigInteger ToBigInteger(this ulong value)
        {
            var parsed = BigInteger.TryParse(value.ToString(), out var parsedValue);
            return !parsed ? BigInteger.Zero : parsedValue;

        }
        public static BigInteger ToBigInteger(this string value)
        {
            var parsed = BigInteger.TryParse(value, out var parsedValue);
            return !parsed ? BigInteger.Zero : parsedValue;
        }

        public static string Add(this string amountA, string amountB)
        {
            return BigInteger.Add(amountA.ToBigInteger(), amountB.ToBigInteger()).ToString();
        }
    }
}