using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions
{
    public class BaseTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly TransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;

        public BaseTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new TransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void BaseTransactionQuoteCommand_InvalidWalletAddress_ThrowArgumentException()
        {
            // Arrange
            Address walletAddress = Address.Empty;

            // Act
            void Act() => new TransactionQuoteCommand(walletAddress);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Wallet address must be provided.");
        }

        [Fact]
        public async Task BaseTransactionQuoteCommand_ExecuteAsync_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            var command = new TransactionQuoteCommand(walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BaseTransactionQuoteCommand_ExecuteAsync_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            var command = new TransactionQuoteCommand(walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(walletAddress, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", FixedDecimal.Zero, "MethodName", _config.WalletTransactionCallback);

            var returnedQuote = new TransactionQuote("1000", null, 23800, null, expectedRequest);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedQuote);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(returnedQuote), Times.Once);
        }

        private class TransactionQuoteCommand : BaseTransactionQuoteCommand
        {
            public TransactionQuoteCommand(Address walletAddress) : base(walletAddress)
            {
            }
        }

        private class TransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<TransactionQuoteCommand>
        {
            public TransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler, IMediator mediator, OpdexConfiguration config)
                : base(quoteAssembler, mediator, config)
            {
            }

            public override async Task<TransactionQuoteDto> Handle(TransactionQuoteCommand request, CancellationToken cancellationToken)
            {
                var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", FixedDecimal.Zero, "MethodName", _callbackEndpoint);

                return await ExecuteAsync(quoteRequest, cancellationToken);
            }
        }
    }
}
