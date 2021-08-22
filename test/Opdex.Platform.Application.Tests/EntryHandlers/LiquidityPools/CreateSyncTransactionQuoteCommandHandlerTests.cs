using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools
{
    public class CreateSyncTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateSyncTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = LiquidityPoolConstants.Methods.Sync;

        public CreateSyncTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateSyncTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateSyncTransactionQuoteCommand_InvalidLiquidityPool_ThrowArgumentException(string liquidityPool)
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            // Act
            void Act() => new CreateSyncTransactionQuoteCommand(liquidityPool, walletAddress);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Liquidity pool must be provided.");
        }

        [Fact]
        public async Task CreateSyncTransactionQuoteCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new CreateSyncTransactionQuoteCommand(liquidityPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(c => c.Address == liquidityPool && c.FindOrThrow == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task CreateSyncTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string crsToSend = "0";

            var command = new CreateSyncTransactionQuoteCommand(liquidityPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                          && c.QuoteRequest.To == liquidityPool
                                                                                          && c.QuoteRequest.Amount == crsToSend
                                                                                          && c.QuoteRequest.Method == MethodName
                                                                                          && c.QuoteRequest.Callback != null),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateSyncTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string crsToSend = "0";

            var command = new CreateSyncTransactionQuoteCommand(liquidityPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(walletAddress, liquidityPool, crsToSend, MethodName, _config.WalletTransactionCallback);

            var expectedQuote = new TransactionQuote("100", null, 23800, null, expectedRequest);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedQuote);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
        }
    }
}
