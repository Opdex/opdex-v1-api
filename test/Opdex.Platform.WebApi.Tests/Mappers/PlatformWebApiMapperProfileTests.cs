using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Mappers;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Mappers
{
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
                Allowance = "500000",
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
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                MinerBalance = "100.00000000",
                TotalSupply = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StartMiningEventResponseModel>(dto);

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
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                MinerBalance = "100.00000000",
                TotalSupply = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StopMiningEventResponseModel>(dto);

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
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                StakerBalance = "100.00000000",
                TotalStaked = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StartStakingEventResponseModel>(dto);

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
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                StakerBalance = "100.00000000",
                TotalStaked = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StopStakingEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StopStakingEvent);
            response.Staker.Should().Be(dto.Staker);
            response.StakerBalance.Should().Be(dto.StakerBalance);
            response.TotalStaked.Should().Be(dto.TotalStaked);
            response.Amount.Should().Be(dto.Amount);
        }

        #endregion
    }
}
