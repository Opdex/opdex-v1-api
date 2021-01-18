namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class NodeStatusDto
    {
        public string Agent { get; set; }
        public string Version { get; set; }
        public string ExternalAddress { get; set; }
        public string Network { get; set; }
        public string CoinTicker { get; set; }
        public long ProcessId { get; set; }
        public ulong ConsensusHeight { get; set; }
        public ulong BlockStoreHeight { get; set; }
        public string DataDirectoryPath { get; set; }
        public string RunningTime { get; set; }
        public long Difficulty { get; set; }
        public long ProtocolVersion { get; set; }
        public bool Testnet { get; set; }
        public decimal RelayFee { get; set; }
        public string State { get; set; }
    }
}