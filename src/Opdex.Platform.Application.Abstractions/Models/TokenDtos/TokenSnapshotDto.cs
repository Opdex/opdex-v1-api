using System;
using Opdex.Platform.Application.Abstractions.Models.OHLC;

namespace Opdex.Platform.Application.Abstractions.Models.TokenDtos
{
    public class TokenSnapshotDto
    {
        public long Id { get; set; }
        public OhlcDecimalDto Price { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}