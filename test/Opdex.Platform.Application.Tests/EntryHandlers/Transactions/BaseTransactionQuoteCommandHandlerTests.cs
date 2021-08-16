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
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new TransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BaseTransactionQuoteCommand_InvalidWalletAddress_ThrowArgumentException(string walletAddress)
        {
            // Arrange
            Address contractAddress = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            // Act
            void Act() => new TransactionQuoteCommand(contractAddress, walletAddress);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Wallet address must be provided.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BaseTransactionQuoteCommand_InvalidContractAddress_ThrowArgumentException(string contractAddress)
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            // Act
            void Act() => new TransactionQuoteCommand(contractAddress, walletAddress);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Contract address must be provided.");
        }

        [Fact]
        public async Task BaseTransactionQuoteCommand_ExecuteAsync_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address contractAddress = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new TransactionQuoteCommand(contractAddress, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                          && c.QuoteRequest.To == contractAddress),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task BaseTransactionQuoteCommand_ExecuteAsync_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address contractAddress = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new TransactionQuoteCommand(contractAddress, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(walletAddress, contractAddress, "0", "MethodName", _config.WalletTransactionCallback);

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
            _assemblerMock.Verify(callTo => callTo.Assemble(It.Is<TransactionQuote>(q => q.Request.Sender == walletAddress
                                                                                    && q.Request.To == contractAddress)), Times.Once);
        }

        private class TransactionQuoteCommand : BaseTransactionQuoteCommand
        {
            public TransactionQuoteCommand(Address contractAddress, Address walletAddress) : base(contractAddress, walletAddress)
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
                var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.ContractAddress, "0", "MethodName", _callbackEndpoint);

                return await ExecuteAsync(quoteRequest, cancellationToken);
            }
        }
    }
}
