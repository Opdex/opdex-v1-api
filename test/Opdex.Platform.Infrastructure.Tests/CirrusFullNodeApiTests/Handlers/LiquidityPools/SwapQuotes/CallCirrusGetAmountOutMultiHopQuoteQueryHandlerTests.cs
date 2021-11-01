using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools.SwapQuotes;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.LiquidityPools.SwapQuotes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.LiquidityPools.SwapQuotes
{
    public class CallCirrusGetAmountOutMultiHopQuoteQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractModule;
        private readonly CallCirrusGetAmountOutMultiHopQuoteQueryHandler _handler;

        public CallCirrusGetAmountOutMultiHopQuoteQueryHandlerTests()
        {
            _smartContractModule = new Mock<ISmartContractsModule>();

            _handler = new CallCirrusGetAmountOutMultiHopQuoteQueryHandler(_smartContractModule.Object);
        }

        [Fact]
        public async Task Handle_LocalCallRequestDto_Send()
        {
            // Arrange
            var query = new CallCirrusGetAmountOutMultiHopQuoteQuery(new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"),
                                                                    5000, 100000000, 2500000000000, 50000000, 50000000000);
            var cancellationToken = new CancellationTokenSource().Token;

            _smartContractModule.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new LocalCallResponseDto());

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            var expectedParameters = new SmartContractMethodParameter[]
            {
                new SmartContractMethodParameter(query.TokenInAmount),
                new SmartContractMethodParameter((UInt256)query.TokenInReserveCrs),
                new SmartContractMethodParameter(query.TokenInReserveSrc),
                new SmartContractMethodParameter((UInt256)query.TokenOutReserveCrs),
                new SmartContractMethodParameter(query.TokenOutReserveSrc),
            };
            _smartContractModule.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(request => request.ContractAddress == query.Router
                                                                                                           && request.Amount == FixedDecimal.Zero
                                                                                                           && request.MethodName == "GetAmountOut"
                                                                                                           && request.GasPrice == 100
                                                                                                           && request.GasLimit == 250_000
                                                                                                           && request.Parameters.SequenceEqual(expectedParameters)
                                                                                                           && request.BlockHeight == null), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsAnErrorMessage_ThrowException()
        {
            // Arrange
            var query = new CallCirrusGetAmountOutMultiHopQuoteQuery(new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"),
                                                                    5000, 100000000, 2500000000000, 50000000, 50000000000);

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
        public async Task Handle_Successful_ReturnAmountOut()
        {
            // Arrange
            var query = new CallCirrusGetAmountOutMultiHopQuoteQuery(new Address("tMCT728cPmhexrrqqkErDbYAC9eA1wNGZA"),
                                                                    5000, 100000000, 2500000000000, 50000000, 50000000000);

            var amountOut = "8000000000000000";
            _smartContractModule.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new LocalCallResponseDto { Return = amountOut });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().Be(UInt256.Parse(amountOut));
        }
    }
}
