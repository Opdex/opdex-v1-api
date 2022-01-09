using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults.Certificates;

public class PersistVaultGovernanceCertificateCommandHandlerTests
{
    private readonly Mock<IDbContext> _dbContextMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PersistVaultGovernanceCertificateCommandHandler _handler;

    public PersistVaultGovernanceCertificateCommandHandlerTests()
    {
        _dbContextMock = new Mock<IDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new PersistVaultGovernanceCertificateCommandHandler(_dbContextMock.Object, Mock.Of<ILogger<PersistVaultGovernanceCertificateCommandHandler>>(), _mapperMock.Object);
    }

    [Fact]
    public async Task Insert_VaultGovernanceCertificate_Success()
    {
        const ulong expectedId = 10ul;
        var certificate = new VaultCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultCertificateEntity>(certificate)).Returns(new VaultCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task Update_VaultCertificate_Success()
    {
        const ulong expectedId = 10ul;
        var certificate = new VaultCertificate(expectedId, 5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, false, false, 50, 100);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultCertificateEntity>(certificate)).Returns(new VaultCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }

    [Fact]
    public async Task PersistsVaultCertificate_Fail()
    {
        const ulong expectedId = 0;
        var certificate = new VaultCertificate(5, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", 500000000000, 100000, 50);
        var command = new PersistVaultGovernanceCertificateCommand(certificate);

        _mapperMock.Setup(callTo => callTo.Map<VaultCertificateEntity>(certificate)).Returns(new VaultCertificateEntity());
        _dbContextMock.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>())).ThrowsAsync(new Exception("Some SQL Exception"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedId);
    }
}
