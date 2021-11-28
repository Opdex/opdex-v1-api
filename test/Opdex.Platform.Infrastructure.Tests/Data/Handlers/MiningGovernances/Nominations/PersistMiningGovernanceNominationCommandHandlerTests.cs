using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances.Nominations
{
    public class PersistMiningGovernanceNominationCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMiningGovernanceNominationCommandHandler _handler;

        public PersistMiningGovernanceNominationCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMiningGovernanceNominationCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMiningGovernanceNominationCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistNew_MiningGovernanceNomination_Success()
        {
            // Arrange
            const ulong expectedId = 10ul;

            var miningGovernance = new MiningGovernanceNomination(3, 4, 5, true, 1000000, 1);
            var command = new PersistMiningGovernanceNominationCommand(miningGovernance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.Is<DatabaseQuery>(q => q.Sql.Contains("INSERT"))))
                .Returns(() => Task.FromResult(expectedId));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistUpdate_MiningGovernanceNomination_Success()
        {
            // Arrange
            const ulong expectedId = 10ul;

            var miningGovernance = new MiningGovernanceNomination(expectedId, 3, 4, 5, true, 1000000, 1, 2);
            var command = new PersistMiningGovernanceNominationCommand(miningGovernance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.Is<DatabaseQuery>(q => q.Sql.Contains("UPDATE"))))
                .Returns(() => Task.FromResult(expectedId));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistMiningGovernanceNomination_Fail()
        {
            // Arrange
            var miningGovernance = new MiningGovernanceNomination(3, 4, 5, true, 1000000, 1);
            var command = new PersistMiningGovernanceNominationCommand(miningGovernance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(() => 0ul);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(0L);
        }
    }
}
