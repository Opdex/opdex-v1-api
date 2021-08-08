using System;
using Newtonsoft.Json;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenSnapshotEntity : SnapshotEntity
    {
        public long Id { get; set; }
        public long MarketId { get; set; }
        public long TokenId { get; set; }
        public OhlcDecimalEntity Price { get; set; }
        public string Details
        {
            get => SerializeSnapshotDetails();
            set => DeserializeSnapshotDetails(value);
        }

        private string SerializeSnapshotDetails()
        {
            return JsonConvert.SerializeObject(Price);
        }

        private void DeserializeSnapshotDetails(string details)
        {
            Price = JsonConvert.DeserializeObject<OhlcDecimalEntity>(details) ??
                    throw new Exception("Invalid token snapshot details.");
        }
    }
}