using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernance;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernance
{
    public class PersistMiningGovernanceCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMiningGovernanceCommandHandler _handler;
    
        public PersistMiningGovernanceCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMiningGovernanceCommandHandler>();
        
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMiningGovernanceCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsMiningGovernance_Success()
        {
            const long expectedId = 1234567;
        
            var miningGovernance = new Domain.MiningGovernance("Address", 1, 2, 4, "12312323", 1, 2);
            var command = new PersistMiningGovernanceCommand(miningGovernance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));
        
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }

        [Fact]
        public async Task PersistsMiningGovernance_Fail()
        {
            var token = new Domain.MiningGovernance("Address", 1, 2, 4, "12312323", 1, 2);
            var command = new PersistMiningGovernanceCommand(token);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
    