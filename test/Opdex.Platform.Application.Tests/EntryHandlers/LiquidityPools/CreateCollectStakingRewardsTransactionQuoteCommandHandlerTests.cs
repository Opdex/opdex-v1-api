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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools
{
    public class CreateCollectStakingRewardsTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateCollectStakingRewardsTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = LiquidityPoolConstants.Methods.CollectStakingRewards;

        public CreateCollectStakingRewardsTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateCollectStakingRewardsTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateCollectStakingRewardsTransactionQuoteCommand_InvalidLiquidityPool_ThrowArgumentException(string liquidityPool)
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const bool liquidate = true;

            // Act
            void Act() => new CreateCollectStakingRewardsTransactionQuoteCommand(liquidityPool, walletAddress, liquidate);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Liquidity pool must be provided.");
        }

        [Fact]
        public async Task CreateCollectStakingRewardsTransactionQuoteCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const bool liquidate = true;

            var command = new CreateCollectStakingRewardsTransactionQuoteCommand(liquidityPool, walletAddress, liquidate);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLiquidityPoolByAddressQuery>(p => p.Address == liquidityPool.ToString()
                                                                                                       && p.FindOrThrow == true), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateCollectStakingRewardsTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string crsToSend = "0";
            const bool liquidate = true;

            var command = new CreateCollectStakingRewardsTransactionQuoteCommand(liquidityPool, walletAddress, liquidate);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Liquidate Rewards", liquidate)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), cancellationToken), Times.Once);
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
        public async Task CreateCollectStakingRewardsTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string crsToSend = "0";
            const bool liquidate = true;

            var command = new CreateCollectStakingRewardsTransactionQuoteCommand(liquidityPool, walletAddress, liquidate);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
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
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), cancellationToken), Times.Once);
        }
    }
}
