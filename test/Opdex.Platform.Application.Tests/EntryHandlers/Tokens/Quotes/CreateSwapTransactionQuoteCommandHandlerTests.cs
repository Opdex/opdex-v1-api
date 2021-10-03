using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Markets;
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
    public class CreateSwapTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateSwapTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;

        public CreateSwapTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateSwapTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        private const string DefaultTokenInAmount = "10.12";
        private UInt256 DefaultTokenInAmountSats = UInt256.Parse("1012000000");
        private const string DefaultTokenOutAmount = "1.892";
        private UInt256 DefaultTokenOutAmountSats = UInt256.Parse("1892000000000000000");
        private const string DefaultTokenInMaximumAmount = "11.00";
        private UInt256 DefaultTokenInMaximumAmountSats = UInt256.Parse("1100000000");
        private const string DefaultTokenOutMinimumAmount = "1.8";
        private UInt256 DefaultTokenOutMinimumAmountSats = UInt256.Parse("1800000000000000000");

        private static CreateSwapTransactionQuoteCommand BuildCommand(string tokenIn = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy",
                                                                      string wallet = "PHJgUKz633jsy1XTenAyWcdTKU64jVFCDo",
                                                                      string tokenOut = "CRS",
                                                                      string tokenInAmount = DefaultTokenInAmount,
                                                                      string tokenOutAmount = DefaultTokenOutAmount,
                                                                      string tokenInMaximumAmount = DefaultTokenInMaximumAmount,
                                                                      string tokenOutMinimumAmount = DefaultTokenOutMinimumAmount,
                                                                      bool tokenInExactAmount = true,
                                                                      string recipient = "P33jsy1XTenAyWcdTKU64jVFCDoHJgUKz6",
                                                                      string market = "PyWcdTKU64jVF1XTenADoHJgUKz6C33jsy",
                                                                      ulong deadline = 0)
        {
            return new CreateSwapTransactionQuoteCommand(tokenIn, wallet, tokenOut, FixedDecimal.Parse(tokenInAmount), FixedDecimal.Parse(tokenOutAmount),
                                                         FixedDecimal.Parse(tokenInMaximumAmount),FixedDecimal.Parse(tokenOutMinimumAmount),
                                                         tokenInExactAmount, recipient, market, deadline);
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenIn_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenIn: null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token in address must be provided.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenOut_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenOut: null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token out address must be provided.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenInAmount_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenInAmount: FixedDecimal.Zero.ToString());

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token in amount must be greater than zero.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenOutAmount_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenOutAmount: FixedDecimal.Zero.ToString());

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token out amount must be greater than zero.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenInMaximumAmount_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenInMaximumAmount: FixedDecimal.Zero.ToString());

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token in maximum amount must be greater than zero.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_TokenOutMinimumAmount_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(tokenOutMinimumAmount: FixedDecimal.Zero.ToString());

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Token out minimum amount must be greater than zero.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_Recipient_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(recipient: null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Recipient address must be provided.");
        }

        [Fact]
        public void CreateSwapTransactionQuoteCommand_Market_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => BuildCommand(market: null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Market address must be provided.");
        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            var command = BuildCommand();
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn && p.FindOrThrow == true),
                                                       cancellationToken), Times.Once);

            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut && p.FindOrThrow == true),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_Sends_RetrieveMarketByAddressQuery()
        {
            // Arrange
            var command = BuildCommand();
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(p => p.Address == command.Market && p.FindOrThrow == true),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_Sends_RetrieveActiveMarketRouterByMarketIdQuery()
        {
            // Arrange
            var command = BuildCommand();
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveActiveMarketRouterByMarketIdQuery>(p => p.MarketId == market.Id && p.FindOrThrow == true),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapExactSrcForCrsTransaction()
        {
            // Arrange
            var command = BuildCommand();
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount In", DefaultTokenInAmountSats),
                new TransactionQuoteRequestParameter("Minimum Amount Out", (ulong)DefaultTokenOutMinimumAmountSats),
                new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapExactSrcForCrs
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapSrcForExactCrsTransaction()
        {
            // Arrange
            var command = BuildCommand(tokenInExactAmount: false);
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount Out", (ulong)DefaultTokenOutAmountSats),
                new TransactionQuoteRequestParameter("Maximum Amount In", DefaultTokenInMaximumAmountSats),
                new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapSrcForExactCrs
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapExactCrsForSrcTransaction()
        {
            // Arrange
            var command = BuildCommand(tokenIn: "CRS", tokenOut: "PoHJgWcdTKUz6UK3jsy1XTenAy364jVFCD");
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Minimum Amount Out", DefaultTokenOutMinimumAmountSats),
                new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == command.TokenInAmount
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapExactCrsForSrc
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapCrsForExactSrcTransaction()
        {
            // Arrange
            var command = BuildCommand(tokenIn: "CRS", tokenOut: "PoHJgWcdTKUz6UK3jsy1XTenAy364jVFCD", tokenInExactAmount: false);
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount Out", DefaultTokenOutAmountSats),
                new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == command.TokenInAmount
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapCrsForExactSrc
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapExactSrcForSrcTransaction()
        {
            // Arrange
            var command = BuildCommand(tokenIn: "PdTKUnAy36oHJgWc4jVFCDz6UK3jsy1XTe", tokenOut: "PoHJgWcdTKUz6UK3jsy1XTenAy364jVFCD");
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 100_000_000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);
            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount In", DefaultTokenInAmountSats),
                new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                new TransactionQuoteRequestParameter("Minimum Amount Out", DefaultTokenOutMinimumAmountSats),
                new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapExactSrcForSrc
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task CreateSwapTransactionQuoteCommand_BuildsAndQuotes_SwapSrcForExactSrcTransaction()
        {
            // Arrange
            var command = BuildCommand(tokenIn: "PdTKUnAy36oHJgWc4jVFCDz6UK3jsy1XTe", tokenOut: "PoHJgWcdTKUz6UK3jsy1XTenAy364jVFCD", tokenInExactAmount: false);
            var cancellationToken = new CancellationTokenSource().Token;

            var tokenIn = new Token(5, command.TokenIn, false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);
            var tokenOut = new Token(5, command.TokenOut, false, "Anything Else", "ANY", 18, 1_000_000_000_000_000_000, UInt256.Parse("210000000000000000"), 5, 15);

            var market = new Market(1, command.Market, 2, 3, Address.Empty, "Pz6364jVFCDoHJgUK3jsy1XTenAyWcdTKU", true, true, true, 3, true, 4, 5);
            var router = new MarketRouter(2, "Pz636HJgUK3jsy1XTenAyWcdTKU4jVFCDo", market.Id, true, 4, 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenIn), cancellationToken)).ReturnsAsync(tokenIn);
            _mediatorMock.Setup(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(p => p.Address == command.TokenOut), cancellationToken)).ReturnsAsync(tokenOut);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), cancellationToken)).ReturnsAsync(market);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken)).ReturnsAsync(router);

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Maximum Amount In", DefaultTokenInMaximumAmountSats),
                new TransactionQuoteRequestParameter("Token In", tokenIn.Address),
                new TransactionQuoteRequestParameter("Amount Out", DefaultTokenOutAmountSats),
                new TransactionQuoteRequestParameter("Token Out", tokenOut.Address),
                new TransactionQuoteRequestParameter("Recipient", command.Recipient),
                new TransactionQuoteRequestParameter("Deadline", command.Deadline)
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == command.WalletAddress
                                                                                               && c.QuoteRequest.To == router.Address
                                                                                               && c.QuoteRequest.Amount == FixedDecimal.Zero
                                                                                               && c.QuoteRequest.Method == RouterConstants.Methods.SwapSrcForExactSrc
                                                                                               && c.QuoteRequest.Callback == _config.WalletTransactionCallback
                                                                                               && c.QuoteRequest.Parameters
                                                                                                   .All(p => expectedParameters
                                                                                                            .Select(e => e.Value)
                                                                                                            .Contains(p.Value))),
                                                       It.IsAny<CancellationToken>()), Times.Once);

        }
    }
}
