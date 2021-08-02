namespace Opdex.Platform.Domain.Models.TransactionLogs.MiningPools
{
    public class StopMiningLog : MineLog
    {
        public StopMiningLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StopMiningLog, (string)log?.miner, (string)log?.amount, (string)log?.totalSupply,
                   (string)log?.minerBalance, address, sortOrder)
        {
        }

        public StopMiningLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StopMiningLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
