using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances;

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
        const ulong expectedId = 10ul;

        var miningGovernance = new MiningGovernance("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 1, 2, 3);
        var command = new PersistMiningGovernanceCommand(miningGovernance);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedId));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsMiningGovernance_Fail()
    {
        var token = new MiningGovernance("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 1, 2, 3);
        var command = new PersistMiningGovernanceCommand(token);

        _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(0ul));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(0);
    }
}