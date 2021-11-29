using System;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotDto
    {
        public OhlcDto<decimal> Price { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
