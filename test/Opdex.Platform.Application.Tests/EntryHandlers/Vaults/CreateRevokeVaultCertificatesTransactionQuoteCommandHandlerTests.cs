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
    public class CreateRevokeVaultCertificatesTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateRevokeVaultCertificatesTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = VaultConstants.Methods.RevokeCertificates;

        public CreateRevokeVaultCertificatesTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration();
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateRevokeVaultCertificatesTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateRevokeVaultCertificatesTransactionQuoteCommand_InvalidVault_ThrowArgumentException()
        {
            // Arrange
            Address vault = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";

            // Act
            void Act() => new CreateRevokeVaultCertificatesTransactionQuoteCommand(vault, walletAddress, vault);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Vault address must be provided.");
        }

        [Fact]
        public void CreateRevokeVaultCertificatesTransactionQuoteCommand_InvalidHolder_ThrowArgumentException()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address vault = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            Address holder = Address.Empty;

            // Act
            void Act() => new CreateRevokeVaultCertificatesTransactionQuoteCommand(vault, walletAddress, holder);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Certificate holder address must be provided.");
        }

        [Fact]
        public async Task CreateRevokeVaultCertificatesTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateRevokeVaultCertificatesTransactionQuoteCommand(vault, walletAddress, holder);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Holder", holder)
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
        public async Task CreateRevokeVaultCertificatesTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address holder = "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm";
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateRevokeVaultCertificatesTransactionQuoteCommand(vault, walletAddress, holder);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Holder", holder)
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
