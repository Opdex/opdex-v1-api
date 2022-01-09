using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults.Certificates;

public class SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler _handler;

    public SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task SelectVaultGovernanceCertificatesByVaultIdAndOwner_Success()
    {
        const ulong vaultId = 10;
        Address owner = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var expected = new List<VaultCertificateEntity>
        {
            new()
            {
                Id = 11,
                VaultId = vaultId,
                Owner = owner,
                Amount = 233,
                VestedBlock = 100,
                Redeemed = false,
                Revoked = true,
                CreatedBlock = 1,
                ModifiedBlock = 2
            },
            new()
            {
                Id = 11,
                VaultId = vaultId,
                Owner = owner,
                Amount = 233,
                VestedBlock = 14,
                Redeemed = true,
                Revoked = false,
                CreatedBlock = 1,
                ModifiedBlock = 2
            }
        };

        var command = new SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vaultId, owner);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultCertificateEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains("vault_certificate"))))
            .ReturnsAsync(expected.ToArray());

        var results = await _handler.Handle(command, CancellationToken.None);
        var resultsList = results.ToList();

        for(var i = 0; i < expected.Count; i++)
        {
            resultsList[i].Id.Should().Be(expected[i].Id);
            resultsList[i].VaultId.Should().Be(expected[i].VaultId);
            resultsList[i].Owner.Should().Be(expected[i].Owner);
            resultsList[i].Amount.Should().Be(expected[i].Amount);
            resultsList[i].VestedBlock.Should().Be(expected[i].VestedBlock);
            resultsList[i].Redeemed.Should().Be(expected[i].Redeemed);
            resultsList[i].Revoked.Should().Be(expected[i].Revoked);
            resultsList[i].CreatedBlock.Should().Be(expected[i].CreatedBlock);
            resultsList[i].ModifiedBlock.Should().Be(expected[i].ModifiedBlock);
        }
    }

    [Fact]
    public async Task SelectVaultGovernanceCertificatesByVaultIdAndOwner_ReturnsEmpty()
    {
        const ulong vaultId = 10;
        Address owner = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var command = new SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery(vaultId, owner);

        _dbContext.Setup(db => db.ExecuteQueryAsync<VaultCertificateEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(Enumerable.Empty<VaultCertificateEntity>);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeEmpty();
    }
}
