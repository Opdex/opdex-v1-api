using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MarketTokens;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MarketTokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MarketTokens;
using Opdex.Platform.Infrastructure.Data.Handlers.MarketTokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MarketTokens
{
    public class SelectMarketTokenByMarketAndTokenIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMarketTokenByMarketAndTokenIdQueryHandler _handler;

        public SelectMarketTokenByMarketAndTokenIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMarketTokenByMarketAndTokenIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTokenById_Success()
        {
            // Arrange
            const ulong id = 99ul;
            const ulong marketId = 3;
            const ulong tokenId = 2;
            var expectedEntity = new MarketTokenEntity()
            {
                Id = id,
                MarketId = marketId,
                TokenId = tokenId,
                CreatedBlock = 10,
                ModifiedBlock = 11
            };

            _dbContext.Setup(db => db.ExecuteFindAsync<MarketTokenEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("FROM market_token"))))
                .ReturnsAsync(expectedEntity);

            // Act
            var result = await _handler.Handle(new SelectMarketTokenByMarketAndTokenIdQuery(marketId, tokenId), CancellationToken.None);

            // Assert
            result.Id.Should().Be(expectedEntity.Id);
            result.MarketId.Should().Be(expectedEntity.MarketId);
            result.TokenId.Should().Be(expectedEntity.TokenId);
            result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
            result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
        }

        [Fact]
        public void SelectTokenById_Throws_NotFoundException()
        {
            // Arrange
            _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("FROM market_token"))))
                .ReturnsAsync(() => null);

            // Act
            // Assert
            _handler.Invoking(h => h.Handle(new SelectMarketTokenByMarketAndTokenIdQuery(1, 2), CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MarketToken)} not found.");
        }

        [Fact]
        public async Task SelectTokenById_ReturnsNull()
        {
            // Arrange
            const bool findOrThrow = false;

            var command = new SelectMarketTokenByMarketAndTokenIdQuery(1, 2, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<TokenEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("FROM market_token"))))
                .ReturnsAsync(() => null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
