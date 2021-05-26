using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningGovernance;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;

namespace Opdex.Platform.Domain.Models
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success, string newContractAddress = null)
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
            Success = success;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
        }

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success, IEnumerable<TransactionLog> logs, string newContractAddress = null)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Success = success;
            NewContractAddress = newContractAddress;
            Logs = new List<TransactionLog>();
            AttachLogs(logs);
        }
        
        public long Id { get; private set; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public bool Success { get; }
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
        
        public void SetId(long id)
        {
            if (Id != 0)
            {
                throw new Exception("TransactionId already set.");
            }

            Id = id;

            foreach (var log in Logs)
            {
                log.SetTransactionId(Id);
            }
        }
        
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
                    nameof(CreateLiquidityPoolLog) => new CreateLiquidityPoolLog(log, address, sortOrder),
                    nameof(CreateMiningPoolLog) => new CreateMiningPoolLog(log, address, sortOrder),
                    nameof(StartStakingLog) => new StartStakingLog(log, address, sortOrder),
                    nameof(StartMiningLog) => new StartMiningLog(log, address, sortOrder),
                    nameof(CollectStakingRewardsLog) => new CollectStakingRewardsLog(log, address, sortOrder),
                    nameof(CollectMiningRewardsLog) => new CollectMiningRewardsLog(log, address, sortOrder),
                    nameof(StopStakingLog) => new StopStakingLog(log, address, sortOrder),
                    nameof(StopMiningLog) => new StopMiningLog(log, address, sortOrder),
                    nameof(RewardMiningPoolLog) => new RewardMiningPoolLog(log, address, sortOrder),
                    nameof(EnableMiningLog) => new EnableMiningLog(log, address, sortOrder),
                    nameof(NominationLog) => new NominationLog(log, address, sortOrder),
                    nameof(ChangeVaultOwnerLog) => new ChangeVaultOwnerLog(log, address, sortOrder),
                    nameof(DistributionLog) => new DistributionLog(log, address, sortOrder),
                    nameof(CreateMarketLog) => new CreateMarketLog(log, address, sortOrder),
                    nameof(ChangeMarketOwnerLog) => new ChangeMarketOwnerLog(log, address, sortOrder),
                    nameof(ChangeMarketPermissionLog) => new ChangeMarketPermissionLog(log, address, sortOrder),
                    nameof(ChangeMarketLog) => new ChangeMarketLog(log, address, sortOrder),
                    nameof(CreateVaultCertificateLog) => new CreateVaultCertificateLog(log, address, sortOrder),
                    nameof(RevokeVaultCertificateLog) => new RevokeVaultCertificateLog(log, address, sortOrder),
                    nameof(RedeemVaultCertificateLog) => new RedeemVaultCertificateLog(log, address, sortOrder),
                    nameof(ChangeDeployerOwnerLog) => new ChangeDeployerOwnerLog(log, address, sortOrder),
                    _ => null // Todo: think about keeping these around incase it is an opdex integrated tx
                };

                if (opdexLog == null) return;

                Logs.Add(opdexLog);
            }
            catch
            {
                // ignored
            }
        }
    }
}