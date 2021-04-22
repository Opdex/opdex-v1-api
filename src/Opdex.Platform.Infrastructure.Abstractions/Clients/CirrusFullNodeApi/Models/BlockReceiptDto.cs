using System.Collections.Generic;
using Newtonsoft.Json;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class BlockReceiptDto
    {
        public string Hash { get; set; }
        public ulong Confirmations { get; set; }
        public ulong Size { get; set; }
        public ulong Weight { get; set; }
        public ulong Height { get; set; }
        public ulong Version { get; set; }
        public string VersionHex { get; set; }
        public string Time { get; set; }
        public string MedianTime { get; set; }
        public long Nonce { get; set; }
        public string Bits { get; set; }
        public ulong Difficulty { get; set; }
        public string Chainwork { get; set; }
        public ulong NTx { get; set; }

        [JsonProperty("previousblockhash")]
        public string PreviousBlockHash { get; set; }

        [JsonProperty("nextblockhash")]
        public string NextBlockHash { get; set; }

        [JsonProperty("merkleroot")]
        public string MerkleRoot { get; set; }

        public IEnumerable<string> Tx { get; set; }
    }
}