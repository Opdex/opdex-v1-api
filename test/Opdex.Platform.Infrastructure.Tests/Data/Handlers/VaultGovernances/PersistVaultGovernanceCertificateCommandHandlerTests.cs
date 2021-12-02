using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.VaultGovernances;

public class PersistVaultGovernanceCertificateCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultGovernanceCertificateCommandHandler _handler;

    public PersistVaultGovernanceCertificateCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultGovernanceCertificateCommandHandler(_dbContextMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_VaultGovernanceCertificate_Success()
    {
        const ulong expectedId = 10ul;
        var certificate = new VaultGovernanceCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultGovernanceCertificateEntity>(certificate)).Returns(new VaultGovernanceCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_VaultGovernanceCertificate_Success()
    {
        const ulong expectedId = 10ul;
        var certificate = new VaultGovernanceCertificate(expectedId, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, false, false, 50, 100);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultGovernanceCertificateEntity>(certificate)).Returns(new VaultGovernanceCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVaultGovernanceCertificate_Fail()
    {
        const ulong expectedId = 0;
        var certificate = new VaultGovernanceCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultGovernanceCertificateEntity>(certificate)).Returns(new VaultGovernanceCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
