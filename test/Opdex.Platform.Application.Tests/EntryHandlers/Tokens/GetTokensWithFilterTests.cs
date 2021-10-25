using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens
{
    public class GetTokensWithFilterTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<Token, TokenDto>> _assemblerMock;

        private readonly GetTokensWithFilterQueryHandler _handler;

        public GetTokensWithFilterTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<Token, TokenDto>>();

            _handler = new GetTokensWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public void GetTokensWithFilter_InvalidCursor_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new GetTokensWithFilterQuery(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Tokens cursor must be set.");
        }

        [Fact]
        public async Task Handle_RetrieveTokensWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new TokensCursor("PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L",
                                          new Address[] { "PAmvCGQNeVVDMbgUkXKprGLzzUCPT9Wqu5", "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u" },
                                          new [] { TokenAttributeType.SRC20, TokenAttributeType.OLPT },
                                          TokenOrderByType.DailyPriceChangePercent,
                                          SortDirectionType.ASC,
                                          25,
                                          PagingDirection.Forward,
                                          ("50.00", 10));
            var request = new GetTokensWithFilterQuery(cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokensWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_TokensRetrieved_MapResults()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 25, PagingDirection.Forward, ("50.00", 10));
            var request = new GetTokensWithFilterQuery(cursor);
            var token = new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10);
            var tokens = new [] { token };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);

            // Act
            try
            {
                await _handler.Handle(request, CancellationToken.None);
            }
            catch (Exception) { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(token), Times.Once);
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 4, PagingDirection.Forward, ("50.00", 10));
            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Tokens.Count().Should().Be(tokens.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 2, PagingDirection.Backward, ("50.00", 10));
            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Tokens.Count().Should().Be(tokens.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 2, PagingDirection.Forward, ("50.00", 10));
            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Tokens.Count().Should().Be(tokens.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 2, PagingDirection.Forward, default);

            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, ("0.23", tokens[1].Id));
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.DailyPriceChangePercent, SortDirectionType.ASC, 2, PagingDirection.Forward, ("10.12", 2));

            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, ("0.23", tokens[^2].Id));
            AssertPrevious(dto.Cursor, ("0.23", tokens[0].Id));
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.Symbol, SortDirectionType.ASC, 2, PagingDirection.Backward, ("10.12", 2));

            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10),
                new Token(3, "fjjc9U4kqdrc4hYPeLPSqkCUMpPykkfL3X", true, "Binance", "BNB", 8, 100_000_000, 3_100_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, (tokens[^1].Symbol, tokens[^1].Id));
            AssertPrevious(dto.Cursor, (tokens[1].Symbol, tokens[1].Id));
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.Name, SortDirectionType.ASC, 2, PagingDirection.Forward, ("10.12", 2));

            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, (tokens[0].Name, tokens[0].Id));
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new TokensCursor(null, Enumerable.Empty<Address>(), Enumerable.Empty<TokenAttributeType>(),
                                          TokenOrderByType.PriceUsd, SortDirectionType.ASC, 2, PagingDirection.Backward, ("10.12", 2));

            var request = new GetTokensWithFilterQuery(cursor);
            var tokens = new []
            {
                new Token(1, "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10),
                new Token(2, "Pefjjc9U4kqdrc4LPSqkCUMpPykkfL3XhY", true, "Ethereum", "ETH", 18, 1_000_000_000_000_000_000, 21_000_000_000_000_000, 9, 10)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokensWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokens);
            int tokenAssembled = 0;
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Token>())).ReturnsAsync(() =>
            {
                var tokenDto = new TokenDto
                {
                    Id = tokens[tokenAssembled].Id, Address = tokens[tokenAssembled].Address, Name = tokens[tokenAssembled].Name,
                    Symbol = tokens[tokenAssembled].Symbol, Decimals = tokens[tokenAssembled].Decimals, Sats = tokens[tokenAssembled].Sats,
                    Summary = new TokenSummaryDto { PriceUsd = 1.11m, DailyPriceChangePercent = 0.23m, ModifiedBlock = 10000 }
                };
                tokenAssembled++;
                return tokenDto;
            });

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, ("1.11", tokens[^1].Id));
            dto.Cursor.Previous.Should().Be(null);
        }

        private static void AssertNext(CursorDto dto, (string, ulong) pointer)
        {
            TokensCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private static void AssertPrevious(CursorDto dto, (string, ulong) pointer)
        {
            TokensCursor.TryParse(dto.Previous.Base64Decode(), out var previous).Should().Be(true);
            previous.PagingDirection.Should().Be(PagingDirection.Backward);
            previous.Pointer.Should().Be(pointer);
        }
    }
}
