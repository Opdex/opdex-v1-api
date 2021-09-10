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
    public class CreateRedeemVaultCertificatesTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateRedeemVaultCertificatesTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = VaultConstants.Methods.RedeemCertificates;

        public CreateRedeemVaultCertificatesTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateRedeemVaultCertificatesTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateRedeemVaultCertificatesTransactionQuoteCommand_InvalidVault_ThrowArgumentException()
        {
            // Arrange
            Address vault = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            // Act
            void Act() => new CreateRedeemVaultCertificatesTransactionQuoteCommand(vault, walletAddress);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Vault address must be provided.");
        }

        [Fact]
        public async Task CreateRedeemVaultCertificatesTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateRedeemVaultCertificatesTransactionQuoteCommand(vault, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>();

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
        public async Task CreateRedeemVaultCertificatesTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address vault = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateRedeemVaultCertificatesTransactionQuoteCommand(vault, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>();

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
