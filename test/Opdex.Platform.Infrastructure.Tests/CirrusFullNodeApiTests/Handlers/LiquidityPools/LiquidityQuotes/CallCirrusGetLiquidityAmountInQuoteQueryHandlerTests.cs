using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.LiquidityQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.LiquidityQuotes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.LiquidityPools.LiquidityQuotes
{
    public class CallCirrusGetLiquidityAmountInQuoteQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractModule;
        private readonly CallCirrusGetLiquidityAmountInQuoteQueryHandler _handler;

        public CallCirrusGetLiquidityAmountInQuoteQueryHandlerTests()
        {
            _smartContractModule = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetLiquidityAmountInQuoteQueryHandler(_smartContractModule.Object);
        }

        [Fact]
        public async Task Handle_LocalCallRequestDto_Send()
        {
            // Arrange
            var query = new CallCirrusGetLiquidityAmountInQuoteQuery(5000, 100000000, 2500000000000, new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"));
            var cancellationToken = new CancellationTokenSource().Token;

            _smartContractModule.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new LocalCallResponseDto());

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            var expectedParameters = new SmartContractMethodParameter[]
            {
                new SmartContractMethodParameter(query.AmountA),
                new SmartContractMethodParameter(query.ReserveA),
                new SmartContractMethodParameter(query.ReserveB)
            };
            _smartContractModule.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(request => request.ContractAddress == query.Router
                                                                                                           && request.Amount == FixedDecimal.Zero
                                                                                                           && request.MethodName == "GetLiquidityQuote"
                                                                                                           && request.GasPrice == 100
                                                                                                           && request.GasLimit == 250_000
                                                                                                           && request.Parameters.SequenceEqual(expectedParameters)
                                                                                                           && request.BlockHeight == null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsAnErrorMessage_ThrowException()
        {
            // Arrange
            var query = new CallCirrusGetLiquidityAmountInQuoteQuery(5000, 100000000, 2500000000000, new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"));

            var errorMessage = "Something went wrong!";
            _smartContractModule.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new LocalCallResponseDto { ErrorMessage = new Error { Value = errorMessage } });

            // Act
            Task Act() => _handler.Handle(query, CancellationToken.None);

            // Assert
            var exception = await Assert.ThrowsAnyAsync<Exception>(Act);
            exception.Message.Should().Contain(errorMessage);
        }

        [Fact]
        public async Task Handle_Successful_ReturnAmountIn()
        {
            // Arrange
            var query = new CallCirrusGetLiquidityAmountInQuoteQuery(5000, 100000000, 2500000000000, new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"));

            var amountIn = "8000000000000000";
            _smartContractModule.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new LocalCallResponseDto { Return = amountIn });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(UInt256.Parse(amountIn));
        }
    }
}
