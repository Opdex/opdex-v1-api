using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;

namespace Opdex.Platform.Domain.Models.Vaults;

public class VaultProposalSummary
{
    public VaultProposalSummary(Address creator, UInt256 amount, Address wallet, byte type, byte status, ulong expiration, ulong yesAmount,
                                ulong noAmount, ulong pledgeAmount)
    {
        var proposalType = (VaultProposalType)type;
        var proposalStatus = (VaultProposalStatus)status;

        Creator = creator != Address.Empty ? creator : throw new ArgumentNullException(nameof(creator), "Creator must be provided.");
        Amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        Wallet = wallet != Address.Empty ? wallet : throw new ArgumentNullException(nameof(wallet), "Wallet must be provided.");
        Type = proposalType.IsValid() ? proposalType : throw new ArgumentOutOfRangeException(nameof(proposalType), "Type must be a valid value.");
        Status = proposalStatus.IsValid() ? proposalStatus : throw new ArgumentOutOfRangeException(nameof(proposalStatus), "Status must be a valid value.");
        Expiration = expiration > 0 ? expiration : throw new ArgumentOutOfRangeException(nameof(expiration), "Expiration must be greater than zero.");
        YesAmount = yesAmount;
        NoAmount = noAmount;
        PledgeAmount = pledgeAmount;
    }

    public Address Creator { get; }
    public UInt256 Amount { get; }
    public Address Wallet { get; }
    public VaultProposalType Type { get; }
    public VaultProposalStatus Status { get; }
    public ulong Expiration { get; }
    public ulong YesAmount { get; }
    public ulong NoAmount { get; }
    public ulong PledgeAmount { get; }
}
