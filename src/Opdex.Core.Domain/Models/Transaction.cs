using System;
using System.Collections.Generic;
using System.Linq;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Domain.Models
{
    public class Transaction
    {
        public Transaction(string txHash, ulong blockHeight, int gasUsed, string from, string to)
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
            
            if (!to.HasValue())
            {
                throw new ArgumentNullException(nameof(to));
            }

            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Logs = new List<TransactionLog>();
        }

        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to, IEnumerable<TransactionLog> logs)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Logs = new List<TransactionLog>();
            AttachLogs(logs);
        }
        
        public Transaction(long id, string txHash, ulong blockHeight, int gasUsed, string from, string to)
        {
            Id = id;
            Hash = txHash;
            BlockHeight = blockHeight;
            GasUsed = gasUsed;
            From = from;
            To = to;
            Logs = new List<TransactionLog>();
        }
        
        public long Id { get; }
        public string Hash { get; }
        public ulong BlockHeight { get; }
        public int GasUsed { get; }
        public string From { get; }
        public string To { get; }
        public ICollection<TransactionLog> Logs { get; }
        public IReadOnlyCollection<string> PoolsEngaged { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolsEngaged"></param>
        /// <exception cref="Exception"></exception>
        public void SetPoolsEngaged(List<string> poolsEngaged)
        {
            if (!poolsEngaged.All(p => Logs.Any(e => e.Address == p)))
            {
                throw new Exception("Pool engagement not found in transaction logs");
            }

            PoolsEngaged = poolsEngaged.Select(p => p).ToList();
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
                    nameof(EnterStakingPoolLog) => new EnterStakingPoolLog(log, address, sortOrder),
                    nameof(EnterMiningPoolLog) => new EnterMiningPoolLog(log, address, sortOrder),
                    nameof(CollectStakingRewardsLog) => new CollectStakingRewardsLog(log, address, sortOrder),
                    nameof(CollectMiningRewardsLog) => new CollectMiningRewardsLog(log, address, sortOrder),
                    nameof(ExitStakingPoolLog) => new ExitStakingPoolLog(log, address, sortOrder),
                    nameof(ExitMiningPoolLog) => new ExitMiningPoolLog(log, address, sortOrder),
                    nameof(RewardMiningPoolLog) => new RewardMiningPoolLog(log, address, sortOrder),
                    nameof(MiningPoolRewardedLog) => new MiningPoolRewardedLog(log, address, sortOrder),
                    nameof(NominationLog) => new NominationLog(log, address, sortOrder),
                    nameof(OwnerChangeLog) => new OwnerChangeLog(log, address, sortOrder),
                    nameof(DistributionLog) => new DistributionLog(log, address, sortOrder),
                    _ => null
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