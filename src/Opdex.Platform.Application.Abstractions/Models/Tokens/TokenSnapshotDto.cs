using Opdex.Platform.Application.Abstractions.Models.OHLC;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotDto
    {
        public ulong Id { get; set; }
        public OhlcDecimalDto Price { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
