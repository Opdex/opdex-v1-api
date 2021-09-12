using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Numerics;

namespace Opdex.Platform.Common.Extensions
{
    /// <summary>
    /// Extension helpers to convert ulong satoshi amounts to decimals and back
    /// </summary>
    public static class SatoshiConverterExtension
    {
        /// <summary>
        /// Converts a token decimal value to its satoshi representation
        /// </summary>
        /// <param name="value">The token decimal value to convert.</param>
        /// <param name="decimals">Decimal precision of the token.</param>
        /// <returns>A <see cref="UInt256" /> representation of the token amount.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <see cref="decimals" /> is greater than 18.</exception>
        /// <exception cref="OverflowException">Thrown if the integral part of <see cref="FixedDecimal" /> exceeds size of `UInt256.MaxValue`.</exception>

        public static UInt256 ToSatoshis(this FixedDecimal value, int decimals)
        {
            if (decimals > 18) throw new ArgumentOutOfRangeException(nameof(decimals), "Tokens over 18 decimals are not supported.");
            var scaledValue = value.Resize((byte)decimals).ScaledValue;
            return (UInt256)scaledValue;
        }

        /// <summary>
        /// Converts a token satoshi value to its decimal representation.
        /// </summary>
        /// <param name="value">The token satoshi value to convert.</param>
        /// <param name="decimals">Decimal precision of the token.</param>
        /// <returns>A <see cref="FixedDecimal" /> representation of the token amount.</returns>
        public static FixedDecimal ToDecimal(this ulong value, int decimals)
        {
            return new FixedDecimal(value, (byte)decimals);
        }

        /// <summary>
        /// Converts a token satoshi value to its decimal representation.
        /// </summary>
        /// <param name="value">The token satoshi value to convert.</param>
        /// <param name="decimals">Decimal precision of the token.</param>
        /// <returns>A <see cref="FixedDecimal" /> representation of the token amount.</returns>
        public static FixedDecimal ToDecimal(this UInt256 value, int decimals)
        {
            return new FixedDecimal(value, (byte)decimals);
        }

        /// <summary>
        /// Retrieves the number of sats per token from a precision value.
        /// </summary>
        /// <param name="decimals">The number of decimals of precision.</param>
        /// <returns>The number of sats per token.</returns>
        public static ulong SatsFromPrecision(int decimals) => (ulong)Math.Pow(10, decimals);

        public static UInt256 Token0PerToken1(this ulong token0, UInt256 token1, ulong token1Sats)
        {
            return ((UInt256)token0).Token0PerToken1(token1, token1Sats);
        }

        public static UInt256 Token0PerToken1(this UInt256 token0, ulong token1, ulong token1Sats)
        {
            return token0.Token0PerToken1((UInt256)token1, token1Sats);
        }

        public static UInt256 Token0PerToken1(this UInt256 token0, UInt256 token1, ulong token1Sats)
        {
            return token1 == UInt256.Zero ? UInt256.Zero : (UInt256)((BigInteger)token0 * token1Sats / token1);
        }
    }
}
