using System;
using Newtonsoft.Json;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenSnapshotEntity : SnapshotEntity
    {
        public ulong Id { get; set; }
        public ulong MarketId { get; set; }
        public ulong TokenId { get; set; }
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
