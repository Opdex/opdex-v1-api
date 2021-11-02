using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    /// <summary>
    /// Represents the reserves of a liquidity pool.
    /// </summary>
    public class ReservesReceipt
    {
        public ReservesReceipt(ulong crs, UInt256 src)
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
