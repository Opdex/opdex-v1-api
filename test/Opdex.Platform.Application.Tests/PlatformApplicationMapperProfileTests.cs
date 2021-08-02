using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Application.Tests
{
    public class PlatformApplicationMapperProfileTests
    {
        private readonly IMapper _mapper;

        public PlatformApplicationMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();
        }

        [Fact]
        public void From_AddressAllowance_To_AddressAllowanceDto()
        {
            // Arrange
            var model = new AddressAllowance(5L, 15L, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", "5000060000", 500, 1000);

            // Act
            var dto = _mapper.Map<AddressAllowanceDto>(model);

            // Assert
            dto.Owner.Should().Be(model.Owner);
            dto.Spender.Should().Be(model.Spender);
        }

        #region Transactions

        [Fact]
        public void From_StartMiningLog_To_StartMiningEventDto()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "10000000000";
            txLog.minerBalance = "20000000000";
            txLog.totalSupply = "30000000000";

            // Act
            var log = new StartMiningLog(1, 2, "Pmdp2uVqojah5kcXzHiBtV8LVDVGVAgv73h", 3, JsonConvert.SerializeObject(txLog));

            // Act
            var response = _mapper.Map<StartMiningEventDto>(log);

            // Assert
            response.Id.Should().Be(log.Id);
            response.TransactionId.Should().Be(log.TransactionId);
            response.Contract.Should().Be(log.Contract);
            response.SortOrder.Should().Be(log.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StartMiningEvent);
            response.Miner.Should().Be(log.Miner);
            response.MinerBalance.Should().Be(log.MinerBalance.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
            response.TotalSupply.Should().Be(log.TotalSupply.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
            response.Amount.Should().Be(log.Amount.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
        }

        [Fact]
        public void From_StopMiningLog_To_StopMiningEventDto()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "10000000000";
            txLog.minerBalance = "20000000000";
            txLog.totalSupply = "30000000000";

            // Act
            var log = new StopMiningLog(1, 2, "Pmdp2uVqojah5kcXzHiBtV8LVDVGVAgv73h", 3, JsonConvert.SerializeObject(txLog));

            // Act
            var response = _mapper.Map<StopMiningEventDto>(log);

            // Assert
            response.Id.Should().Be(log.Id);
            response.TransactionId.Should().Be(log.TransactionId);
            response.Contract.Should().Be(log.Contract);
            response.SortOrder.Should().Be(log.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StopMiningEvent);
            response.Miner.Should().Be(log.Miner);
            response.MinerBalance.Should().Be(log.MinerBalance.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
            response.TotalSupply.Should().Be(log.TotalSupply.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
            response.Amount.Should().Be(log.Amount.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals));
        }

        [Fact]
        public void From_StartStakingLog_To_StartStakingEventDto()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "10000000000";
            txLog.stakerBalance = "20000000000";
            txLog.totalStaked = "30000000000";

            // Act
            var log = new StartStakingLog(1, 2, "Pmdp2uVqojah5kcXzHiBtV8LVDVGVAgv73h", 3, JsonConvert.SerializeObject(txLog));

            // Act
            var response = _mapper.Map<StartStakingEventDto>(log);

            // Assert
            response.Id.Should().Be(log.Id);
            response.TransactionId.Should().Be(log.TransactionId);
            response.Contract.Should().Be(log.Contract);
            response.SortOrder.Should().Be(log.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StartStakingEvent);
            response.Staker.Should().Be(log.Staker);
            response.StakerBalance.Should().Be(log.StakerBalance.InsertDecimal(TokenConstants.Opdex.Decimals));
            response.TotalStaked.Should().Be(log.TotalStaked.InsertDecimal(TokenConstants.Opdex.Decimals));
            response.Amount.Should().Be(log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals));
        }

        [Fact]
        public void From_StopStakingLog_To_StopStakingEventDto()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.staker = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "10000000000";
            txLog.stakerBalance = "20000000000";
            txLog.totalStaked = "30000000000";

            // Act
            var log = new StopStakingLog(1, 2, "Pmdp2uVqojah5kcXzHiBtV8LVDVGVAgv73h", 3, JsonConvert.SerializeObject(txLog));

            // Act
            var response = _mapper.Map<StopStakingEventDto>(log);

            // Assert
            response.Id.Should().Be(log.Id);
            response.TransactionId.Should().Be(log.TransactionId);
            response.Contract.Should().Be(log.Contract);
            response.SortOrder.Should().Be(log.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StopStakingEvent);
            response.Staker.Should().Be(log.Staker);
            response.StakerBalance.Should().Be(log.StakerBalance.InsertDecimal(TokenConstants.Opdex.Decimals));
            response.TotalStaked.Should().Be(log.TotalStaked.InsertDecimal(TokenConstants.Opdex.Decimals));
            response.Amount.Should().Be(log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals));
        }

        #endregion
    }
}
