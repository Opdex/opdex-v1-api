using Opdex.Platform.WebApi.Models.Responses.OHLC;
using System;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class TokenSnapshotResponseModel
    {
        public ulong Id { get; set; }
        public OhlcDecimalResponseModel Price { get; set; }
        public decimal? DailyPriceChange { get; set; }
        public int SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
