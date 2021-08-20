using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
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
    public class CreateCreateLiquidityPoolTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateCreateLiquidityPoolTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = LiquidityPoolConstants.Methods.CreateLiquidityPool;

        public CreateCreateLiquidityPoolTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateCreateLiquidityPoolTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateCreateLiquidityPoolTransactionQuoteCommand_InvalidAmount_ThrowArgumentNullException()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address token = null;

            // Act
            void Act() => new CreateCreateLiquidityPoolTransactionQuoteCommand(liquidityPool, walletAddress, token);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token must be provided.");
        }

        [Fact]
        public async Task CreateCreateLiquidityPoolTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address token = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";
            const string crsToSend = "0";

            var command = new CreateCreateLiquidityPoolTransactionQuoteCommand(liquidityPool, walletAddress, token);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Token Address", token)
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
        public async Task CreateCreateLiquidityPoolTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address token = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGJcuwA";
            const string crsToSend = "0";

            var command = new CreateCreateLiquidityPoolTransactionQuoteCommand(liquidityPool, walletAddress, token);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Token Address", token)
            };

            var expectedRequest = new TransactionQuoteRequest(walletAddress, liquidityPool, crsToSend, MethodName, _config.WalletTransactionCallback, expectedParameters);

            var expectedQuote = new TransactionQuote("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjQf", null, 23800, null, expectedRequest);

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
