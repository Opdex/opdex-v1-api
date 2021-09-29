using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets
{
    public class PersistMarketCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMarketCommandHandler _handler;

        public PersistMarketCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMarketCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMarketCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_Market_Success()
        {
            const long expectedId = 10;
            var market = new Market("PMGLf8N8QEnKoncvGxcDZYoTNG5ysnxXpX", 1, 2, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, true, true, 3, false, 4);
            var command = new PersistMarketCommand(market);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Update_Market_Success()
        {
            const long expectedId = 10;
            var market = new Market(expectedId, "PMGLf8N8QEnKoncvGxcDZYoTNG5ysnxXpX", 1, 2, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, true, true, 3, false, 4, 5);
            var command = new PersistMarketCommand(market);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistsMarket_Fail()
        {
            const long expectedId = 0;
            var market = new Market(expectedId, "PMGLf8N8QEnKoncvGxcDZYoTNG5ysnxXpX", 1, 2, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, true, true, 3, false, 4, 5);
            var command = new PersistMarketCommand(market);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
    }
}
