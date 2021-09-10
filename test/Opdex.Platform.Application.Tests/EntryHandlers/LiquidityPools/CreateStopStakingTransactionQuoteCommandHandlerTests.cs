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
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools
{
    public class CreateStopStakingTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateStopStakingTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = LiquidityPoolConstants.Methods.StopStaking;

        public CreateStopStakingTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateStopStakingTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateStopStakingTransactionQuoteCommand_InvalidLiquidityPool_ThrowArgumentException()
        {
            // Arrange
            Address liquidityPool = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            const bool liquidate = true;

            // Act
            void Act() => new CreateStopStakingTransactionQuoteCommand(liquidityPool, walletAddress, amount, liquidate);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Liquidity pool must be provided.");
        }

        [Fact]
        public async Task CreateStopStakingTransactionQuoteCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            const bool liquidate = true;

            var command = new CreateStopStakingTransactionQuoteCommand(liquidityPool, walletAddress, amount, liquidate);
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
        public async Task CreateStopStakingTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            FixedDecimal crsToSend = FixedDecimal.Zero;
            const bool liquidate = true;

            var command = new CreateStopStakingTransactionQuoteCommand(liquidityPool, walletAddress, amount, liquidate);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("100000000")),
                new TransactionQuoteRequestParameter("Liquidate Rewards", liquidate)
            };

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
                                                                                          && c.QuoteRequest.Callback != null
                                                                                          && c.QuoteRequest.Parameters
                                                                                              .All(p => expectedParameters
                                                                                                       .Select(e => e.Value)
                                                                                                       .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStopStakingTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            FixedDecimal crsToSend = FixedDecimal.Zero;
            const bool liquidate = true;

            var command = new CreateStopStakingTransactionQuoteCommand(liquidityPool, walletAddress, amount, liquidate);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("100000000")),
                new TransactionQuoteRequestParameter("Liquidate Rewards", liquidate)
            };

            var expectedRequest = new TransactionQuoteRequest(walletAddress, liquidityPool, crsToSend, MethodName, _config.WalletTransactionCallback, expectedParameters);

            var expectedQuote = new TransactionQuote("1000", null, 23800, null, expectedRequest);

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
