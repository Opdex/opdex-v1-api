using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Common.Models
{
    /// <summary>
    /// Represents the reserves of a liquidity pool.
    /// </summary>
    public readonly struct Reserves
    {
        public Reserves(ulong crs, UInt256 src)
        {
            Crs = crs;
            Src = src;
        }

        /// <summary>
        /// The amount of CRS tokens in reserves.
        /// </summary>
        public ulong Crs { get; }

        /// <summary>
        /// The amount of SRC tokens in reserves.
        /// </summary>
        public UInt256 Src { get; }
    }
}
