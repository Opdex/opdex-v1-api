using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

namespace Opdex.Platform.Domain.Models
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success, string newContractAddress = null)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash), "Transaction hash must be set.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than 0.");
            }

            if (gasUsed == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gasUsed), "Transaction gas must be set.");
            }

            if (!from.HasValue())
            {
                throw new ArgumentNullException(nameof(from), "From address must be set.");
            }

            if (!to.HasValue() && !newContractAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(to), "To address must be set.");
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

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, bool success, string newContractAddress, List<TransactionLog> logs = null)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Success = success;
            NewContractAddress = newContractAddress;

            if (logs == null)
            {
                Logs = new List<TransactionLog>();
            }
            else
            {
                AttachLogs(logs);
            }
        }

        public long Id { get; private set; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public bool Success { get; }
        public string NewContractAddress { get; }

        public List<TransactionLog> Logs { get; }

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
            foreach (var txLog in logs.OrderBy(log => log.SortOrder))
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
                throw new InvalidOperationException("TransactionId already set.");
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
                    // Deployers
                    nameof(ChangeDeployerOwnerLog) => new ChangeDeployerOwnerLog(log, address, sortOrder),
                    nameof(CreateMarketLog) => new CreateMarketLog(log, address, sortOrder),

                    // Markets
                    nameof(ChangeMarketOwnerLog) => new ChangeMarketOwnerLog(log, address, sortOrder),
                    nameof(ChangeMarketPermissionLog) => new ChangeMarketPermissionLog(log, address, sortOrder),
                    nameof(CreateLiquidityPoolLog) => new CreateLiquidityPoolLog(log, address, sortOrder),

                    // Liquidity Pools
                    nameof(ReservesLog) => new ReservesLog(log, address, sortOrder),
                    nameof(BurnLog) => new BurnLog(log, address, sortOrder),
                    nameof(MintLog) => new MintLog(log, address, sortOrder),
                    nameof(SwapLog) => new SwapLog(log, address, sortOrder),
                    nameof(StakeLog) => new StakeLog(log, address, sortOrder),
                    nameof(CollectStakingRewardsLog) => new CollectStakingRewardsLog(log, address, sortOrder),

                    // Mining Pools
                    nameof(MineLog) => new MineLog(log, address, sortOrder),
                    nameof(CollectMiningRewardsLog) => new CollectMiningRewardsLog(log, address, sortOrder),
                    nameof(EnableMiningLog) => new EnableMiningLog(log, address, sortOrder),

                    // Tokens
                    nameof(ApprovalLog) => new ApprovalLog(log, address, sortOrder),
                    nameof(TransferLog) => new TransferLog(log, address, sortOrder),
                    nameof(DistributionLog) => new DistributionLog(log, address, sortOrder),

                    // Governances
                    nameof(NominationLog) => new NominationLog(log, address, sortOrder),
                    nameof(RewardMiningPoolLog) => new RewardMiningPoolLog(log, address, sortOrder),

                    // Vaults
                    nameof(ChangeVaultOwnerLog) => new ChangeVaultOwnerLog(log, address, sortOrder),
                    nameof(CreateVaultCertificateLog) => new CreateVaultCertificateLog(log, address, sortOrder),
                    nameof(RevokeVaultCertificateLog) => new RevokeVaultCertificateLog(log, address, sortOrder),
                    nameof(RedeemVaultCertificateLog) => new RedeemVaultCertificateLog(log, address, sortOrder),

                    // Else
                    _ => null
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
