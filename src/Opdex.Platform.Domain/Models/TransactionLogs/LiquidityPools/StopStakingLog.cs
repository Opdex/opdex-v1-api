namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class StopStakingLog : StakeLog
    {
        public StopStakingLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StopStakingLog, (string)log?.staker, (string)log?.amount, (string)log?.totalStaked,
                   (string)log?.stakerBalance, address, sortOrder)
        {
        }

        public StopStakingLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StopStakingLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
