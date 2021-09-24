using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Tokens
{
    public class CallCirrusGetStandardTokenContractSummaryQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetStandardTokenContractSummaryQueryHandler _handler;

        public CallCirrusGetStandardTokenContractSummaryQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetStandardTokenContractSummaryQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact]
        public void CallCirrusGetStandardTokenContractSummaryQuery_InvalidTokenAddress_ThrowsArgumentNullException()
        {
            // Arrange
            Address token = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetStandardTokenContractSummaryQuery(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
        }

        [Fact]
        public void CallCirrusGetStandardTokenContractSummaryQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetStandardTokenContractSummaryQuery(token, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetStandardTokenContractSummaryQuery_IncludeBaseProperties_Sends_LocalCalls()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const ulong blockHeight = 10;
            const bool includeBaseProperties = true;
            const bool includeTotalSupply = false;

            const string name = "Bitcoin";
            const string symbol = "BTC";
            const uint decimals = 8;

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Name}"),
                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = name });

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Symbol}"),
                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = symbol });

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Decimals}"),
                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = decimals });

            // Act
            await _handler.Handle(new CallCirrusGetStandardTokenContractSummaryQuery(token, blockHeight, includeBaseProperties, includeTotalSupply), CancellationToken.None);


            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Name}" &&
                                                                                                             q.BlockHeight == blockHeight &&
                                                                                                             q.ContractAddress == token),
                                                                             It.IsAny<CancellationToken>()), Times.Once);

            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Symbol}" &&
                                                                                                             q.BlockHeight == blockHeight &&
                                                                                                             q.ContractAddress == token),
                                                                             It.IsAny<CancellationToken>()), Times.Once);

            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Decimals}" &&
                                                                                                             q.BlockHeight == blockHeight &&
                                                                                                             q.ContractAddress == token),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetStandardTokenContractSummaryQuery_IncludeTotalSupply_Sends_LocalCalls()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const ulong blockHeight = 10;
            const bool includeBaseProperties = false;
            const bool includeTotalSupply = true;

            UInt256 totalSupply = 2_100_000_000_000_000;

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.TotalSupply}"),
                                                                            It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = totalSupply });

            // Act
            await _handler.Handle(new CallCirrusGetStandardTokenContractSummaryQuery(token, blockHeight, includeBaseProperties, includeTotalSupply), CancellationToken.None);

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.TotalSupply}" &&
                                                                                                             q.BlockHeight == blockHeight &&
                                                                                                             q.ContractAddress == token),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetStandardTokenContractSummaryQuery_Returns_StandardTokenContractSummary()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const ulong blockHeight = 10;
            const bool includeBaseProperties = true;
            const bool includeTotalSupply = true;

            const string name = "Bitcoin";
            const string symbol = "BTC";
            const uint decimals = 8;
            UInt256 totalSupply = 2_100_000_000_000_000;
            const ulong expectedSats = 100_000_000;
            const bool expectedIsLpt = false;

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Name}"),
                                                                             It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = name });

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Symbol}"),
                                                                             It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = symbol });

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.Decimals}"),
                                                                             It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = decimals });

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.MethodName == $"get_{StandardTokenConstants.Properties.TotalSupply}"),
                                                                             It.IsAny<CancellationToken>())).ReturnsAsync(new LocalCallResponseDto { Return = totalSupply });

            // Act
            var response = await _handler.Handle(new CallCirrusGetStandardTokenContractSummaryQuery(token, blockHeight, includeBaseProperties, includeTotalSupply), CancellationToken.None);

            // Assert
            response.Name.Should().Be(name);
            response.Symbol.Should().Be(symbol);
            response.Decimals.Should().Be(decimals);
            response.TotalSupply.Should().Be(totalSupply);
            response.Sats.Should().Be(expectedSats);
            response.IsLpt.Should().Be(expectedIsLpt);
        }
    }
}
