using FluentAssertions;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults;

public class VaultProposalCertificateTests
{
    [Fact]
    public void CreateNewVaultProposalCertificate_InvalidProposalId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong proposalId = 0;
        const ulong certificateId = 2;

        //Act
        void Act() => new VaultProposalCertificate(proposalId, certificateId);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Proposal Id must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalCertificate_InvalidCertificateId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong proposalId = 1;
        const ulong certificateId = 0;

        //Act
        void Act() => new VaultProposalCertificate(proposalId, certificateId);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Certificate Id must be greater than zero.");
    }

    [Fact]
    public void CreateNewVaultProposalCertificate_Success()
    {
        // Arrange
        const ulong proposalId = 1;
        const ulong certificateId = 2;

        //Act
        var proposalCertificate = new VaultProposalCertificate(proposalId, certificateId);

        // Assert
        proposalCertificate.Id.Should().Be(0ul);
        proposalCertificate.ProposalId.Should().Be(proposalId);
        proposalCertificate.CertificateId.Should().Be(certificateId);
    }

    [Fact]
    public void CreateVaultProposalCertificate_Success()
    {
        // Arrange
        const ulong id = 99;
        const ulong proposalId = 1;
        const ulong certificateId = 2;

        //Act
        var proposalCertificate = new VaultProposalCertificate(id, proposalId, certificateId);

        // Assert
        proposalCertificate.Id.Should().Be(id);
        proposalCertificate.ProposalId.Should().Be(proposalId);
        proposalCertificate.CertificateId.Should().Be(certificateId);
    }
}
