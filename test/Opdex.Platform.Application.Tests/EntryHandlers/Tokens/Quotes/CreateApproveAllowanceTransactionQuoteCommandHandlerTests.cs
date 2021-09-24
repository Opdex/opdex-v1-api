using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens.Quotes
{
    public class CreateApproveAllowanceTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateApproveAllowanceTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;

        public CreateApproveAllowanceTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateApproveAllowanceTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateApproveAllowanceTransactionQuoteCommand_InvalidToken_ThrowArgumentNullException()
        {
            // Arrange
            Address token = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address spender = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            // Act
            void Act() => new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token address must be provided.");
        }

        [Fact]
        public void CreateApproveAllowanceTransactionQuoteCommand_InvalidSpender_ThrowArgumentNullException()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = Address.Empty;
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            // Act
            void Act() => new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Spender address must be provided.");
        }

        [Fact]
        public void CreateApproveAllowanceTransactionQuoteCommand_InvalidAmount_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU";
            FixedDecimal amount = FixedDecimal.Parse("-1.1");

            // Act
            void Act() => new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount must be greater than or equal to 0.");
        }

        [Fact]
        public async Task CreateApproveAllowanceTransactionQuoteCommand_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU";
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            var command = new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == token &&
                                                                                               p.FindOrThrow == true),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateApproveAllowanceTransactionQuoteCommand_Sends_RetrieveAddressAllowanceQuery()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU";
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            var command = new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock.Setup(callTo => callTo.Send(new RetrieveTokenByAddressQuery(token, true), cancellationToken))
                .ReturnsAsync(new Token(1, token, false, "Bitcoin", "BTC", 8, 100_000_000, 0, 1, 2));

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowanceQuery>(p => p.Owner == walletAddress &&
                                                                                                 p.Spender == spender &&
                                                                                                 p.Token == token),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateApproveAllowanceTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU";
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            var command = new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
                .ReturnsAsync(new Token(1, token, false, "Bitcoin", "BTC", 8, 100_000_000, 0, 1, 2));

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceQuery>(), cancellationToken))
                .ReturnsAsync(new AddressAllowance(1, walletAddress, spender, UInt256.Zero, 1));

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Spender", spender),
                new TransactionQuoteRequestParameter("Current Allowance", UInt256.Zero),
                new TransactionQuoteRequestParameter("New Allowance", amount.ToSatoshis(8))
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                               && c.QuoteRequest.To == token
                                                                                               && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                               && c.QuoteRequest.Method == StandardTokenConstants.Methods.Approve
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateApproveAllowanceTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address token = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address walletAddress = "Pz633jsy1XTenAyWcdTKU64jVFCDoHJgUK";
            Address spender = "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU";
            FixedDecimal amount = FixedDecimal.Parse("1.1");

            var command = new CreateApproveAllowanceTransactionQuoteCommand(token, walletAddress, spender, amount);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(walletAddress, token, FixedDecimal.Zero,
                                                              StandardTokenConstants.Methods.Approve, _config.WalletTransactionCallback);

            var expectedQuote = new TransactionQuote(null, null, 23800, null, expectedRequest);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
                .ReturnsAsync(new Token(1, token, false, "Bitcoin", "BTC", 8, 100_000_000, 0, 1, 2));

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceQuery>(), cancellationToken))
                .ReturnsAsync(new AddressAllowance(1, walletAddress, spender, UInt256.Zero, 1));

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
