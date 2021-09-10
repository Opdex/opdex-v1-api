using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Vaults;
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

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class CreateSetPendingVaultOwnershipTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateSetPendingVaultOwnershipTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = VaultConstants.Methods.SetPendingOwnership;

        public CreateSetPendingVaultOwnershipTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateSetPendingVaultOwnershipTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateSetPendingVaultOwnershipTransactionQuoteCommand_InvalidVault_ThrowArgumentException()
        {
            // Arrange
            Address vault = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address newOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";

            // Act
            void Act() => new CreateSetPendingVaultOwnershipTransactionQuoteCommand(vault, walletAddress, newOwner);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Vault address must be provided.");
        }

        [Fact]
        public void CreateSetPendingVaultOwnershipTransactionQuoteCommand_InvalidNewOwner_ThrowArgumentException()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address vault = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            Address newOwner = Address.Empty;

            // Act
            void Act() => new CreateSetPendingVaultOwnershipTransactionQuoteCommand(vault, walletAddress, newOwner);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("New owner address must be provided.");
        }

        [Fact]
        public async Task CreateSetPendingVaultOwnershipTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address newOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateSetPendingVaultOwnershipTransactionQuoteCommand(vault, walletAddress, newOwner);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("New Owner", newOwner)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                          && c.QuoteRequest.To == vault
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
        public async Task CreateSetPendingVaultOwnershipTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address newOwner = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateSetPendingVaultOwnershipTransactionQuoteCommand(vault, walletAddress, newOwner);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("New Owner", newOwner)
            };

            var expectedRequest = new TransactionQuoteRequest(walletAddress, vault, crsToSend, MethodName, _config.WalletTransactionCallback, expectedParameters);

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
