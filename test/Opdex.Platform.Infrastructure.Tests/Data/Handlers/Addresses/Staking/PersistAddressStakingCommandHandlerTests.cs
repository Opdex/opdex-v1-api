using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Staking
{
    public class PersistAddressStakingCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistAddressStakingCommandHandler _handler;

        public PersistAddressStakingCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistAddressStakingCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistAddressStakingCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_AddressStaking_Success()
        {
            const long expectedId = 10;
            var staking = new AddressStaking(1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3);
            var command = new PersistAddressStakingCommand(staking);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Update_AddressStaking_Success()
        {
            const long expectedId = 10;
            var staking = new AddressStaking(expectedId, 1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3, 4);
            var command = new PersistAddressStakingCommand(staking);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistsAddressStaking_Fail()
        {
            const long expectedId = 0;
            var staking = new AddressStaking(expectedId, 1, "PUFLuoW2K4PgJZ4nt5fEUHfvQXyQWKG9hm", 100000000, 3, 4);
            var command = new PersistAddressStakingCommand(staking);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception("Some SQL Exception"));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
    }
}
