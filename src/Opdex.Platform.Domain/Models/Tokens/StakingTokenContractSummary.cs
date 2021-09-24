using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Domain.Models.Tokens
{
    public class StakingTokenContractSummary
    {
        public StakingTokenContractSummary(ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
        public ulong? Genesis { get; private set; }
        public ulong? PeriodDuration { get; private set; }
        public uint? PeriodIndex { get; private set; }
        public Address? MiningGovernance { get; private set; }
        public Address? Vault { get; private set; }

        public void SetGenesis(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.UInt64)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Genesis value must be of UInt64 type.");
            }

            Genesis = value.Parse<ulong>();
        }

        public void SetPeriodIndex(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.UInt32)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Period index value must be of UInt32 type.");
            }

            PeriodIndex = value.Parse<uint>();
        }

        public void SetPeriodDuration(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.UInt64)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Period duration value must be of UInt64 type.");
            }

            PeriodDuration = value.Parse<ulong>();
        }

        public void SetMiningGovernance(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.Address)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Mining governance value must be of Address type.");
            }

            MiningGovernance = value.Parse<Address>();
        }

        public void SetVault(SmartContractMethodParameter value)
        {
            if (value.Type != SmartContractParameterType.Address)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Vault value must be of Address type.");
            }

            Vault = value.Parse<Address>();
        }
    }
}
