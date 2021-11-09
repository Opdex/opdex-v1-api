using Opdex.Platform.Application.Abstractions.Models.OHLC;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotDto
    {
        public OhlcDecimalDto Price { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
