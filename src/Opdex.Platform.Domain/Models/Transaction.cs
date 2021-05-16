using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Domain.Models
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to, string newContractAddress = null)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }
            
            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }
            
            if (gasUsed == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasUsed));
            }
            
            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from));
            }
            
            if (!to.HasValue() && !newContractAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
        }

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, IEnumerable<TransactionLog> logs, string newContractAddress = null)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
            AttachLogs(logs);
        }
        
        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, string newContractAddress = null)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
        }
        
        public long Id { get; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public string NewContractAddress { get; }
        
        public ICollection<TransactionLog> Logs { get; }
        
        // Todo: maybe enum - unknown | maybe | yeah | no
        public bool IsOpdexTx { get; private set; }

        public List<T> LogsOfType<T>(TransactionLogType logType) where T : TransactionLog
        {
            return Logs.Where(log => log.LogType == logType).Select(log => log as T).ToList();
        }

        public IEnumerable<TransactionLog> LogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return Logs.Where(log => logTypes.Contains(log.LogType)).ToList();
        }

        public IDictionary<string, List<TransactionLog>> GroupedLogsOfTypes(IEnumerable<TransactionLogType> logTypes)
        {
            return LogsOfTypes(logTypes)
                .GroupBy(log => log.Contract)
                .ToDictionary(k => k.FirstOrDefault()?.Contract, logs => logs.ToList());
        }

        private void AttachLogs(IEnumerable<TransactionLog> logs)
        {
            foreach (var txLog in logs)
            {
                if (txLog.TransactionId == 0)
                {
                    txLog.SetTransactionId(Id);
                }

                Logs.Add(txLog);
            }
        }
        
        public void AttachLog(TransactionLog transactionLog)
        {
            if (Id == 0 || transactionLog.Id == 0)
            {
                throw new Exception($"Unable to add transaction log on incomplete {nameof(Transaction)}.");
            }

            // Todo: Insert into sorted order
            Logs.Add(transactionLog);
        }

        // Todo: Use TransactionLogType
        // In general this is ugly
        public void DeserializeLog(string address, string topic, int sortOrder, dynamic log)
        {
            try
            {
                TransactionLog opdexLog = topic switch
                {
                    nameof(ReservesLog) => new ReservesLog(log, address, sortOrder),
                    nameof(BurnLog) => new BurnLog(log, address, sortOrder),
                    nameof(MintLog) => new MintLog(log, address, sortOrder),
                    nameof(SwapLog) => new SwapLog(log, address, sortOrder),
                    nameof(ApprovalLog) => new ApprovalLog(log, address, sortOrder),
                    nameof(TransferLog) => new TransferLog(log, address, sortOrder),
                    nameof(LiquidityPoolCreatedLog) => new LiquidityPoolCreatedLog(log, address, sortOrder),
                    nameof(MiningPoolCreatedLog) => new MiningPoolCreatedLog(log, address, sortOrder),
                    nameof(StartStakingLog) => new StartStakingLog(log, address, sortOrder),
                    nameof(StartMiningLog) => new StartMiningLog(log, address, sortOrder),
                    nameof(CollectStakingRewardsLog) => new CollectStakingRewardsLog(log, address, sortOrder),
                    nameof(CollectMiningRewardsLog) => new CollectMiningRewardsLog(log, address, sortOrder),
                    nameof(StopStakingLog) => new StopStakingLog(log, address, sortOrder),
                    nameof(StopMiningLog) => new StopMiningLog(log, address, sortOrder),
                    nameof(RewardMiningPoolLog) => new RewardMiningPoolLog(log, address, sortOrder),
                    nameof(MiningPoolRewardedLog) => new MiningPoolRewardedLog(log, address, sortOrder),
                    nameof(NominationLog) => new NominationLog(log, address, sortOrder),
                    nameof(ChangeVaultOwnerLog) => new ChangeVaultOwnerLog(log, address, sortOrder),
                    nameof(DistributionLog) => new DistributionLog(log, address, sortOrder),
                    nameof(MarketCreatedLog) => new MarketCreatedLog(log, address, sortOrder),
                    nameof(MarketOwnerChangeLog) => new MarketOwnerChangeLog(log, address, sortOrder),
                    nameof(PermissionsChangeLog) => new PermissionsChangeLog(log, address, sortOrder),
                    nameof(MarketChangeLog) => new MarketChangeLog(log, address, sortOrder),
                    nameof(CreateVaultCertificateLog) => new CreateVaultCertificateLog(log, address, sortOrder),
                    nameof(UpdateVaultCertificateLog) => new UpdateVaultCertificateLog(log, address, sortOrder),
                    nameof(RedeemVaultCertificateLog) => new RedeemVaultCertificateLog(log, address, sortOrder),
                    _ => null // Todo: think about keeping these around incase it is an opdex integrated tx
                };

                if (opdexLog == null) return;

                Logs.Add(opdexLog);
            }
            catch
            {
                // ignored
                // Maybe we want to keep this around incase other logs in this transaction
                // are Opdex logs
            }
        }
    }
}