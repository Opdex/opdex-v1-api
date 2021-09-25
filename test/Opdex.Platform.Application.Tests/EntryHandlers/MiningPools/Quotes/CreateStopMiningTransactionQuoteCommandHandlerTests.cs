using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;
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

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools.Quotes
{
    public class CreateStopMiningTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateStopMiningTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = MiningPoolConstants.Methods.StopMining;

        public CreateStopMiningTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateStopMiningTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateStopMiningTransactionQuoteCommand_InvalidMiningPool_ThrowArgumentNullException()
        {
            // Arrange
            Address miningPool = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");

            // Act
            void Act() => new CreateStopMiningTransactionQuoteCommand(miningPool, walletAddress, amount);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining pool address must be set.");
        }

        [Theory]
        [InlineData("-1.01")]
        [InlineData("0")]
        public void CreateStopMiningTransactionQuoteCommand_InvalidAmount_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            // Act
            void Act() => new CreateStopMiningTransactionQuoteCommand(miningPool, walletAddress, FixedDecimal.Parse(amount));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount must be greater than 0.");
        }

        [Fact]
        public async Task CreateStopMiningTransactionQuoteCommand_Sends_RetrieveMiningPoolByAddressQuery()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");

            var command = new CreateStopMiningTransactionQuoteCommand(miningPool, walletAddress, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByAddressQuery>(c => c.Address == miningPool && c.FindOrThrow == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStopMiningTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateStopMiningTransactionQuoteCommand(miningPool, walletAddress, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("100000000"))
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
                                                                                          && c.QuoteRequest.Method == MethodName
                                                                                          && c.QuoteRequest.Callback != null
                                                                                          && c.QuoteRequest.Parameters
                                                                                              .All(p => expectedParameters
                                                                                                       .Select(e => e.Value)
                                                                                                       .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateStopMiningTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.00");
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateStopMiningTransactionQuoteCommand(miningPool, walletAddress, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("100000000"))
            };

            var expectedRequest = new TransactionQuoteRequest(walletAddress, miningPool, crsToSend, MethodName, _config.WalletTransactionCallback, expectedParameters);

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
