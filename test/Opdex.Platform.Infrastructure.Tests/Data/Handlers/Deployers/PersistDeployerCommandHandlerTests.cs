using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Deployers
{
    public class PersistDeployerCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistDeployerCommandHandler _handler;

        public PersistDeployerCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistDeployerCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistDeployerCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_Deployer_Success()
        {
            const long expectedId = 10;
            const bool isActive = true;

            var deployer = new Deployer("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", isActive, 1);
            var command = new PersistDeployerCommand(deployer);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task Update_Deployer_Success()
        {
            const long expectedId = 10;
            const bool isActive = true;

            var deployer = new Deployer(expectedId, "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", isActive, 1, 2);
            var command = new PersistDeployerCommand(deployer);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistsDeployer_Fail()
        {
            const long expectedId = 0;
            const bool isActive = true;

            var deployer = new Deployer(expectedId, "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", isActive, 1, 2);
            var command = new PersistDeployerCommand(deployer);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
    }
}
