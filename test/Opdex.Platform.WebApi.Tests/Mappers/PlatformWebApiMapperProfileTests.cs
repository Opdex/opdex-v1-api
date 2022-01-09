using AutoMapper;
using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.WebApi.Mappers;
using Opdex.Platform.WebApi.Models.Responses.MiningGovernances;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Linq;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Mappers;

public class PlatformWebApiMapperProfileTests
{
    private readonly IMapper _mapper;

    public PlatformWebApiMapperProfileTests()
    {
        _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformWebApiMapperProfile())).CreateMapper();
    }

    [Fact]
    public void From_AddressAllowanceDto_To_ApprovedAllowanceResponseModel()
    {
        // Arrange
        var dto = new AddressAllowanceDto
        {
            Allowance = FixedDecimal.Parse("500000"),
            Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            Spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            Token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXM",
        };

        // Act
        var response = _mapper.Map<ApprovedAllowanceResponseModel>(dto);

        // Assert
        response.Allowance.Should().Be(dto.Allowance);
        response.Owner.Should().Be(dto.Owner);
        response.Spender.Should().Be(dto.Spender);
        response.Token.Should().Be(dto.Token);
    }

    #region Transactions

    [Fact]
    public void From_StartMiningEventDto_To_StartMiningResponseModel()
    {
        // Arrange
        var dto = new StartMiningEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Amount = FixedDecimal.Parse("100.00000000"),
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            MinerBalance = FixedDecimal.Parse("100.00000000"),
            TotalSupply = FixedDecimal.Parse("234.00000000")
        };

        // Act
        var response = _mapper.Map<StartMiningEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.StartMiningEvent);
        response.Miner.Should().Be(dto.Miner);
        response.MinerBalance.Should().Be(dto.MinerBalance);
        response.TotalSupply.Should().Be(dto.TotalSupply);
        response.Amount.Should().Be(dto.Amount);
    }

    [Fact]
    public void From_StopMiningEventDto_To_StopMiningResponseModel()
    {
        // Arrange
        var dto = new StopMiningEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Amount = FixedDecimal.Parse("100.00000000"),
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            MinerBalance = FixedDecimal.Parse("100.00000000"),
            TotalSupply = FixedDecimal.Parse("234.00000000")
        };

        // Act
        var response = _mapper.Map<StopMiningEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.StopMiningEvent);
        response.Miner.Should().Be(dto.Miner);
        response.MinerBalance.Should().Be(dto.MinerBalance);
        response.TotalSupply.Should().Be(dto.TotalSupply);
        response.Amount.Should().Be(dto.Amount);
    }

    [Fact]
    public void From_StartStakingEventDto_To_StartStakingResponseModel()
    {
        // Arrange
        var dto = new StartStakingEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Amount = FixedDecimal.Parse("100.00000000"),
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            StakerBalance = FixedDecimal.Parse("100.00000000"),
            TotalStaked = FixedDecimal.Parse("234.00000000")
        };

        // Act
        var response = _mapper.Map<StartStakingEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.StartStakingEvent);
        response.Staker.Should().Be(dto.Staker);
        response.StakerBalance.Should().Be(dto.StakerBalance);
        response.TotalStaked.Should().Be(dto.TotalStaked);
        response.Amount.Should().Be(dto.Amount);
    }

    [Fact]
    public void From_StopStakingEventDto_To_StopStakingResponseModel()
    {
        // Arrange
        var dto = new StopStakingEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Amount = FixedDecimal.Parse("100.00000000"),
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            StakerBalance = FixedDecimal.Parse("100.00000000"),
            TotalStaked = FixedDecimal.Parse("234.00000000")
        };

        // Act
        var response = _mapper.Map<StopStakingEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.StopStakingEvent);
        response.Staker.Should().Be(dto.Staker);
        response.StakerBalance.Should().Be(dto.StakerBalance);
        response.TotalStaked.Should().Be(dto.TotalStaked);
        response.Amount.Should().Be(dto.Amount);
    }

    [Fact]
    public void From_AddLiquidityEventDto_To_AddLiquidityResponseModel()
    {
        // Arrange
        var dto = new AddLiquidityEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            AmountCrs = FixedDecimal.Parse("100.00000000"),
            AmountSrc = FixedDecimal.Parse("200.00000000"),
            AmountLpt = FixedDecimal.Parse("300.00000000"),
            TokenSrc = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            TokenLp = "PVguEK5irNcbk7ZfvJtSJgyGvV5JqMgWVr",
            TokenLpTotalSupply = FixedDecimal.Parse("400.00000000"),
        };

        // Act
        var response = _mapper.Map<AddLiquidityEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.AddLiquidityEvent);
        response.AmountCrs.Should().Be(dto.AmountCrs);
        response.AmountSrc.Should().Be(dto.AmountSrc);
        response.AmountLpt.Should().Be(dto.AmountLpt);
        response.TokenSrc.Should().Be(dto.TokenSrc);
        response.TokenLp.Should().Be(dto.TokenLp);
        response.TokenLpTotalSupply.Should().Be(dto.TokenLpTotalSupply);
    }

    [Fact]
    public void From_RemoveLiquidityEventDto_To_RemoveLiquidityResponseModel()
    {
        // Arrange
        var dto = new RemoveLiquidityEventDto
        {
            Id = 1,
            TransactionId = 2,
            SortOrder = 1,
            Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
            AmountCrs = FixedDecimal.Parse("100.00000000"),
            AmountSrc = FixedDecimal.Parse("200.00000000"),
            AmountLpt = FixedDecimal.Parse("300.00000000"),
            TokenSrc = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
            TokenLp = "PVguEK5irNcbk7ZfvJtSJgyGvV5JqMgWVr",
            TokenLpTotalSupply = FixedDecimal.Parse("400.00000000"),
        };

        // Act
        var response = _mapper.Map<RemoveLiquidityEvent>(dto);

        // Assert
        response.Contract.Should().Be(dto.Contract);
        response.SortOrder.Should().Be(dto.SortOrder);
        response.EventType.Should().Be(TransactionEventType.RemoveLiquidityEvent);
        response.AmountCrs.Should().Be(dto.AmountCrs);
        response.AmountSrc.Should().Be(dto.AmountSrc);
        response.AmountLpt.Should().Be(dto.AmountLpt);
        response.TokenSrc.Should().Be(dto.TokenSrc);
        response.TokenLp.Should().Be(dto.TokenLp);
        response.TokenLpTotalSupply.Should().Be(dto.TokenLpTotalSupply);
    }

    #endregion

    [Fact]
    public void From_CertificateDto_To_CertificateResponseModel()
    {
        // Arrange
        var dto = new VaultCertificateDto
        {
            Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXM",
            Amount = FixedDecimal.Parse("5021.24920000"),
            VestingStartBlock = 10002,
            VestingEndBlock = 141842,
            Redeemed = false,
            Revoked = true
        };

        // Act
        var response = _mapper.Map<VaultCertificateResponseModel>(dto);

        // Assert
        response.Owner.Should().Be(dto.Owner);
        response.Amount.Should().Be(dto.Amount);
        response.VestingStartBlock.Should().Be(dto.VestingStartBlock);
        response.VestingEndBlock.Should().Be(dto.VestingEndBlock);
        response.Redeemed.Should().Be(dto.Redeemed);
        response.Revoked.Should().Be(dto.Revoked);
    }

    [Fact]
    public void From_CertificatesDto_To_CertificatesResponseModel()
    {
        // Arrange
        var dto = new VaultCertificatesDto
        {
            Certificates = new VaultCertificateDto[] { new VaultCertificateDto(), new VaultCertificateDto(), new VaultCertificateDto() },
            Cursor = new CursorDto { Next = "aG9sZGVyOjtkaXJlY3Rpb246QVNDO2xpbWl0OjI7cGFnaW5nOkZvcndhcmQ7cG9pbnRlcjpNZz09Ow", Previous = "aG9sZGVyOjtkaXJlY3Rpb246QVNDO2xpbWl0OjI7cGFnaW5nOkZvcndhcmQ7cG9pbnRlcjpNZz09Ow==" }
        };

        // Act
        var response = _mapper.Map<VaultCertificatesResponseModel>(dto);

        // Assert
        response.Results.Count().Should().Be(dto.Certificates.Count());
        response.Paging.Next.Should().Be(dto.Cursor.Next);
        response.Paging.Previous.Should().Be(dto.Cursor.Previous);
    }

    [Fact]
    public void From_MiningPositionDto_To_MiningPositionResponseModel()
    {
        // Arrange
        var dto = new MiningPositionDto
        {
            Address = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK",
            Amount = FixedDecimal.Parse("200.00000000"),
            MiningPool = "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX",
            MiningToken = "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L"
        };

        // Act
        var response = _mapper.Map<MiningPositionResponseModel>(dto);

        // Assert
        response.Address.Should().Be(dto.Address);
        response.Amount.Should().Be(dto.Amount);
        response.MiningPool.Should().Be(dto.MiningPool);
        response.MiningToken.Should().Be(dto.MiningToken);
    }

    [Fact]
    public void From_MiningPoolDto_To_MiningPoolResponseModel()
    {
        // Arrange
        var dto = new MiningPoolDto
        {
            Address = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK",
            LiquidityPool = "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
            IsActive = true,
            MiningPeriodEndBlock = 500_000,
            RewardPerBlock = UInt256.Parse("1666666666"),
            RewardPerLpt = UInt256.Parse("028882888"),
            TokensMining = UInt256.Parse("50038209382219139")
        };

        // Act
        var response = _mapper.Map<MiningPoolResponseModel>(dto);

        // Assert
        response.Address.Should().Be(dto.Address);
        response.LiquidityPool.Should().Be(dto.LiquidityPool);
        response.IsActive.Should().Be(dto.IsActive);
        response.MiningPeriodEndBlock.Should().Be(dto.MiningPeriodEndBlock);
        response.RewardPerBlock.Should().Be(FixedDecimal.Parse("16.66666666"));
        response.RewardPerLpt.Should().Be(FixedDecimal.Parse("0.28882888"));
        response.TokensMining.Should().Be(FixedDecimal.Parse("500382093.82219139"));
    }

    [Fact]
    public void From_MiningGovernanceDto_To_MiningGovernanceResponseModel()
    {
        // Arrange
        var dto = new MiningGovernanceDto
        {
            Address = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK",
            MinedToken = "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
            MiningPoolRewardPerPeriod = FixedDecimal.Parse("1500.00000000"),
            PeriodBlockDuration = 10,
            PeriodEndBlock = 500_000,
            PeriodRemainingBlocks = 10_000,
            PeriodsUntilRewardReset = 20,
            TotalRewardsPerPeriod = FixedDecimal.Parse("3000.00000000")
        };

        // Act
        var response = _mapper.Map<MiningGovernanceResponseModel>(dto);

        // Assert
        response.Address.Should().Be(dto.Address);
        response.MinedToken.Should().Be(dto.MinedToken);
        response.MiningPoolRewardPerPeriod.Should().Be(dto.MiningPoolRewardPerPeriod);
        response.PeriodBlockDuration.Should().Be(dto.PeriodBlockDuration);
        response.PeriodEndBlock.Should().Be(dto.PeriodEndBlock);
        response.PeriodRemainingBlocks.Should().Be(dto.PeriodRemainingBlocks);
        response.PeriodsUntilRewardReset.Should().Be(dto.PeriodsUntilRewardReset);
        response.TotalRewardsPerPeriod.Should().Be(dto.TotalRewardsPerPeriod);
    }
}
