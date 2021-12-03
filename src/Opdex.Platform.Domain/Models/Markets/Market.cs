using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;

namespace Opdex.Platform.Domain.Models.Markets;

public class Market : BlockAudit
{
    public Market(Address address, ulong deployerId, ulong stakingTokenId, Address owner, bool authPoolCreators, bool authProviders,
                  bool authTraders, uint transactionFee, bool marketFeeEnabled, ulong createdBlock) : base(createdBlock)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), "Address must be set.");
        }

        if (deployerId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(deployerId), "Deployer id must be greater than 0.");
        }

        if (owner == Address.Empty)
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

    public Market(ulong id, Address address, ulong deployerId, ulong stakingTokenId, Address pendingOwner, Address owner, bool authPoolCreators, bool authProviders,
                  bool authTraders, uint transactionFee, bool marketFeeEnabled, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        Address = address;
        DeployerId = deployerId;
        StakingTokenId = stakingTokenId;
        PendingOwner = pendingOwner;
        Owner = owner;
        AuthPoolCreators = authPoolCreators;
        AuthProviders = authProviders;
        AuthTraders = authTraders;
        TransactionFee = transactionFee;
        MarketFeeEnabled = marketFeeEnabled;
    }

    public ulong Id { get; }
    public Address Address { get; }
    public ulong DeployerId { get; }
    public ulong StakingTokenId { get; }
    public Address PendingOwner { get; private set; }
    public Address Owner { get; private set; }
    public bool AuthPoolCreators { get; }
    public bool AuthProviders { get; }
    public bool AuthTraders { get; }
    public uint TransactionFee { get; }
    public bool MarketFeeEnabled { get; }
    public bool IsStakingMarket => StakingTokenId > 0;

    public void SetPendingOwnership(SetPendingMarketOwnershipLog log, ulong block)
    {
        if (log is null) throw new ArgumentNullException(nameof(log));

        PendingOwner = log.To;
        SetModifiedBlock(block);
    }

    public void SetOwnershipClaimed(ClaimPendingMarketOwnershipLog log, ulong blockHeight)
    {
        if (log is null) throw new ArgumentNullException(nameof(log));

        PendingOwner = Address.Empty;
        Owner = log.To;
        SetModifiedBlock(blockHeight);
    }

    public void Update(MarketContractSummary summary)
    {
        if (summary is null) throw new ArgumentNullException(nameof(summary));

        if (summary.PendingOwner.HasValue) PendingOwner = summary.PendingOwner.Value;
        if (summary.Owner.HasValue) Owner = summary.Owner.Value;
        SetModifiedBlock(summary.BlockHeight);
    }
}