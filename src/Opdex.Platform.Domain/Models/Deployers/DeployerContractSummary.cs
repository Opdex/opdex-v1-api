using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.Deployers;

public class DeployerContractSummary
{
    public DeployerContractSummary(ulong blockHeight)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");

        BlockHeight = blockHeight;
    }

    public ulong BlockHeight { get; }
    public Address? PendingOwner { get; private set; }
    public Address? Owner { get; private set; }

    public void SetPendingOwner(SmartContractMethodParameter value)
    {
        if (value.Type != SmartContractParameterType.Address)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Pending owner value must be of Address type.");
        }

        var pendingOwner = value.Parse<Address>();

        PendingOwner = pendingOwner;
    }

    public void SetOwner(SmartContractMethodParameter value)
    {
        if (value.Type != SmartContractParameterType.Address)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Owner value must be of Address type.");
        }

        var owner = value.Parse<Address>();

        Owner = owner;
    }
}