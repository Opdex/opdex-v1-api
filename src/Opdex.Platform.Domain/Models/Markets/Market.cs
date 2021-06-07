using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class Market : BlockAudit
    {
        public Market(string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders,
            bool authTraders, uint transactionFee, bool marketFeeEnabled, ulong createdBlock) : base(createdBlock)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address), "Address must be set.");
            }

            if (deployerId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(deployerId), "Deployer id must be greater than 0.");
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner), "Owner must be set.");
            }

            if (transactionFee > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(transactionFee), "Transaction fee must be between 0-10 inclusive.");
            }

            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            TransactionFee = transactionFee;
            MarketFeeEnabled = marketFeeEnabled;
        }

        public Market(long id, string address, long deployerId, long? stakingTokenId, string owner, bool authPoolCreators, bool authProviders,
            bool authTraders, uint transactionFee, bool marketFeeEnabled, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            Address = address;
            DeployerId = deployerId;
            StakingTokenId = stakingTokenId;
            Owner = owner;
            AuthPoolCreators = authPoolCreators;
            AuthProviders = authProviders;
            AuthTraders = authTraders;
            TransactionFee = transactionFee;
            MarketFeeEnabled = marketFeeEnabled;
        }

        public long Id { get; }
        public string Address { get; }
        public long DeployerId { get; }
        public long? StakingTokenId { get; }
        public string Owner { get; private set; }
        public bool AuthPoolCreators { get; }
        public bool AuthProviders { get; }
        public bool AuthTraders { get; }
        public uint TransactionFee { get; }
        public bool MarketFeeEnabled { get; }

        public void SetOwner(ChangeMarketOwnerLog log, ulong blockHeight)
        {
            SetModifiedBlock(blockHeight);
            Owner = log.To;
        }
    }
}