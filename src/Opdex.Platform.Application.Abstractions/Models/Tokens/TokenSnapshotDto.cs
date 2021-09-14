using Opdex.Platform.Application.Abstractions.Models.OHLC;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenSnapshotDto
    {
        public long Id { get; set; }
        public OhlcDecimalDto Price { get; set; }
        public decimal? DailyPriceChange { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public void SetDailyPriceChange(decimal previousPrice)
        {
            DailyPriceChange = Price.Close.PercentChange(previousPrice);
        }
    }
}
