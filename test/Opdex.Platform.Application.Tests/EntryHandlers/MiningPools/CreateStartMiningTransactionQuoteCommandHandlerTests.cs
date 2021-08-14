using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools
{
    public class CreateStartMiningTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateStartMiningTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;

        public CreateStartMiningTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateStartMiningTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("123")]
        [InlineData("asdf")]
        public void CreateStartMiningTransactionQuoteCommand_InvalidAmount_ThrowArgumentException(string amount)
        {
            // Arrange
            const string walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            // Act
            void Act() => new CreateStartMiningTransactionQuoteCommand(walletAddress, amount, miningPool);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Amount must be a valid decimal number.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateStartMiningTransactionQuoteCommand_InvalidMiningPool_ThrowArgumentNullException(string miningPool)
        {
            // Arrange
            const string walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string amount = "1.00";

            // Act
            void Act() => new CreateStartMiningTransactionQuoteCommand(walletAddress, amount, miningPool);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining pool must be provided.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateStartMiningTransactionQuoteCommand_InvalidWalletAddress_ThrowArgumentNullException(string walletAddress)
        {
            // Arrange
            const string miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "1.00";

            // Act
            void Act() => new CreateStartMiningTransactionQuoteCommand(walletAddress, amount, miningPool);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Wallet address must be provided.");
        }

        [Fact]
        public async Task CreateStartMiningTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            const string walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "1.00";
            const string crsToSend = "0";
            const string methodName = MiningPoolConstants.Methods.StartMining;

            var command = new CreateStartMiningTransactionQuoteCommand(walletAddress, amount, miningPool);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", "100000000", SmartContractParameterType.UInt256)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                          && c.QuoteRequest.To == miningPool
                                                                                          && c.QuoteRequest.Amount == crsToSend
                                                                                          && c.QuoteRequest.Method == methodName
                                                                                          && c.QuoteRequest.Callback != null
                                                                                          && c.QuoteRequest.Parameters
                                                                                              .All(p => expectedParameters
                                                                                                       .Select(e => e.Serialized)
                                                                                                       .Contains(p.Serialized))),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStartMiningTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            const string walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            const string miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string amount = "1.00";
            const string crsToSend = "0";
            const string methodName = MiningPoolConstants.Methods.StartMining;

            var command = new CreateStartMiningTransactionQuoteCommand(walletAddress, amount, miningPool);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", "100000000", SmartContractParameterType.UInt256)
            };

            var expectedRequest = new TransactionQuoteRequest(walletAddress, miningPool, crsToSend, methodName, _config.WalletTransactionCallback, expectedParameters);

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
