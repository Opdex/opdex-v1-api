using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Markets.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets.Quotes
{
    public class CreateSetStandardMarketPermissionsTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OpdexConfiguration _config;
        private readonly CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler _handler;

        public CreateSetStandardMarketPermissionsTransactionQuoteCommandHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _mediatorMock = new Mock<IMediator>();
            _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
            _handler = new CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public async Task Handle_RetrieveMarketByAddressQuery_FindOrThrow()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", MarketPermissionType.SetPermissions, true);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                            false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == marketAddress
                                                                                                 && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MarketIsStakingMarket_ThrowNotFoundException()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", MarketPermissionType.SetPermissions, true);
            var cancellationToken = new CancellationTokenSource().Token;

            var stakingMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 15, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                           false, false, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(stakingMarket);

            // Act
            Task Act() => _handler.Handle(command, cancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(Act);
            exception.Message.Should().Be("Market address must represent a standard market.");
        }

        [Fact]
        public async Task Handle_MarketDoesNotAuthPoolCreators_ThrowInvalidDataException()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", MarketPermissionType.CreatePool, true);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                           false, true, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            Task Act() => _handler.Handle(command, cancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("Permission");
            exception.Message.Should().Be("Market does not enforce authorization for pool creation.");
        }

        [Fact]
        public async Task Handle_MarketDoesNotAuthProviders_ThrowInvalidDataException()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", MarketPermissionType.Provide, true);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                           true, false, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            Task Act() => _handler.Handle(command, cancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("Permission");
            exception.Message.Should().Be("Market does not enforce authorization for providing liquidity.");
        }

        [Fact]
        public async Task Handle_MarketDoesNotAuthTraders_ThrowInvalidDataException()
        {
            // Arrange
            Address marketAddress = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", MarketPermissionType.Trade, true);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                           true, true, false, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            // Act
            Task Act() => _handler.Handle(command, cancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("Permission");
            exception.Message.Should().Be("Market does not enforce authorization for trading.");
        }

        [Fact]
        public async Task CreateSetStandardMarketPermissionsTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address marketAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address authority = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address user = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
            MarketPermissionType permission = MarketPermissionType.Trade;
            bool authorize = true;
            FixedDecimal amount = 50;

            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, authority, user, permission, authorize);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                            true, true, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("User", new SmartContractMethodParameter(command.User)),
                new TransactionQuoteRequestParameter("Permission", new SmartContractMethodParameter((byte)command.Permission)),
                new TransactionQuoteRequestParameter("Authorize", new SmartContractMethodParameter(command.Authorize))
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == authority
                                                                                            && c.QuoteRequest.To == marketAddress
                                                                                            && c.QuoteRequest.Amount == crsToSend
                                                                                            && c.QuoteRequest.Method == StandardMarketConstants.Methods.Authorize
                                                                                            && c.QuoteRequest.Callback != null
                                                                                            && c.QuoteRequest.Parameters.All(p => expectedParameters.Select(e => e.Value).Contains(p.Value))),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSetStandardMarketPermissionsTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address marketAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address authority = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address user = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
            MarketPermissionType permission = MarketPermissionType.Provide;
            bool authorize = true;
            FixedDecimal amount = 50;

            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(marketAddress, authority, user, permission, authorize);
            var cancellationToken = new CancellationTokenSource().Token;

            var standardMarket = new Market(5, new Address("PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3"), 10, 0, Address.Empty, new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                                            true, true, true, 3, true, 5, 50);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(standardMarket);

            var expectedRequest = new TransactionQuoteRequest(marketAddress, authority, FixedDecimal.Zero, StandardMarketConstants.Methods.Authorize, _config.WalletTransactionCallback);

            var expectedQuote = new TransactionQuote("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjQf", null, 23800, null, expectedRequest);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedQuote);

            // Act
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            }
            catch { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
        }
    }
}
