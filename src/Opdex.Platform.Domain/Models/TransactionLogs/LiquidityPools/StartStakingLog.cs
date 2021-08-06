namespace Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools
{
    public class StartStakingLog : StakeLog
    {
        public StartStakingLog(dynamic log, string address, int sortOrder)
            : base(TransactionLogType.StartStakingLog, (string)log?.staker, (string)log?.amount, (string)log?.totalStaked,
                   (string)log?.stakerBalance, address, sortOrder)
        {
        }

        public StartStakingLog(long id, long transactionId, string address, int sortOrder, string details)
            : base(TransactionLogType.StartStakingLog, id, transactionId, address, sortOrder, details)
        {
        }
    }
}
