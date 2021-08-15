using FluentAssertions;
using Newtonsoft.Json;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Application.Tests.Mappers
{
    public class TransactionsPlatformApplicationMapperProfile : PlatformApplicationMapperProfileTests
    {
        [Fact]
        public void From_TransactionQuote_To_TransactionQuoteDto()
        {
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "0";
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var quoteRequest = new TransactionQuoteRequest(sender, to, amount, method, callback);
            var transactionQuote = new TransactionQuote("result", "error", 10, null, quoteRequest);

            // Act
            var dto = _mapper.Map<TransactionQuoteDto>(transactionQuote);

            // Assert
            dto.Error.Should().Be("error");
            dto.Result.Should().Be("result");
            dto.GasUsed.Should().Be(10);
            dto.Request.Should().NotBeNull();
        }

        [Fact]
        public void From_TransactionQuoteRequest_To_TransactionQuoteRequestDto()
        {
            // Arrange
            const string sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "1.1";
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter> { new TransactionQuoteRequestParameter("Amount", "1000", SmartContractParameterType.UInt256) };

            var quoteRequest = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

            // Act
            var dto = _mapper.Map<TransactionQuoteRequestDto>(quoteRequest);

            // Assert
            dto.Sender.Should().Be(sender);
            dto.To.Should().Be(to);
            dto.Amount.Should().Be(amount);
            dto.Method.Should().Be(method);
            dto.Callback.Should().Be(callback);
            dto.Parameters.Select(p => p.Value).Should().BeEquivalentTo(parameters.Select(p => p.Serialized));
        }

        [Fact]
        public void From_TransactionQuoteRequestParameter_To_TransactionQuoteRequestParameterDto()
        {
            // Arrange
            var parameter = new TransactionQuoteRequestParameter("Amount", "1000", SmartContractParameterType.UInt256);

            // Act
            var dto = _mapper.Map<TransactionQuoteRequestParameterDto>(parameter);

            // Assert
            dto.Label.Should().Be(parameter.Label);
            dto.Value.Should().Be(parameter.Serialized);
        }

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
    }
}
