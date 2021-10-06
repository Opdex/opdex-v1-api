using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Market;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens.Market
{
    public class PersistMarketTokenCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMarketTokenCommandHandler _handler;

        public PersistMarketTokenCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMarketTokenCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMarketTokenCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public void PersistMarketToken_InvalidMarketToken_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new PersistMarketTokenCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Market token must be provided.");
        }

        [Fact]
        public async Task PersistMarketToken_Success()
        {
            // Arrange
            var token = new MarketToken(1, 2, 3);
            var command = new PersistMarketTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public async Task PersistMarketToken_Fail()
        {
            // Arrange
            var token = new MarketToken(1, 2, 3);
            var command = new PersistMarketTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(false);
        }
    }
}
