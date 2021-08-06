namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class StartMiningLog : MineLog
    {
        public StartMiningLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StartMiningLog, (string)log?.miner, (string)log?.amount, (string)log?.totalSupply,
                   (string)log?.minerBalance, address, sortOrder)
        {
        }

        public StartMiningLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StartMiningLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
