using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Mining
{
    public class PersistAddressMiningCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistAddressMiningCommandHandler _handler;

        public PersistAddressMiningCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistAddressMiningCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistAddressMiningCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_AddressMining_Success()
        {
            const ulong expectedId = 10ul;
            var mining = new AddressMining(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 100000000, 3);
            var command = new PersistAddressMiningCommand(mining);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Update_AddressMining_Success()
        {
            const ulong expectedId = 10ul;
            var mining = new AddressMining(expectedId, 1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 100000000, 3, 4);
            var command = new PersistAddressMiningCommand(mining);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistsAddressMining_Fail()
        {
            const ulong expectedId = 0;
            var mining = new AddressMining(expectedId, 1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 100000000, 3, 4);
            var command = new PersistAddressMiningCommand(mining);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception("Some SQL Exception"));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
    }
}
