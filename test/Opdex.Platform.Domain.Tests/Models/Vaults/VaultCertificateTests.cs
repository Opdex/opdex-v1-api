using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Vaults;

public class VaultCertificateTests
{
    [Fact]
    public void CreateNewVaultCertificate_InvalidVaultId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 0;
        Address owner = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 amount = 2;
        const ulong vestedBlock = 3;
        const ulong createdBlock = 4;

        //Act
        void Act() => new VaultCertificate(vaultId, owner, amount, vestedBlock, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Vault id must be greater than 0.");
    }

    [Fact]
    public void CreateNewVaultCertificate_InvalidOwner_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong vaultId = 1;
        Address owner = "";
        UInt256 amount = 2;
        const ulong vestedBlock = 3;
        const ulong createdBlock = 4;

        //Act
        void Act() => new VaultCertificate(vaultId, owner, amount, vestedBlock, createdBlock);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be set.");
    }

    [Fact]
    public void CreateNewVaultCertificate_InvalidVestedBlock_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        Address owner = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 amount = 2;
        const ulong vestedBlock = 0;
        const ulong createdBlock = 4;

        //Act
        void Act() => new VaultCertificate(vaultId, owner, amount, vestedBlock, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Vested block must be greater than 0.");
    }

    [Fact]
    public void CreateNewVaultCertificate_InvalidAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const ulong vaultId = 1;
        Address owner = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 amount = 0;
        const ulong vestedBlock = 3;
        const ulong createdBlock = 4;

        //Act
        void Act() => new VaultCertificate(vaultId, owner, amount, vestedBlock, createdBlock);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount must be greater than 0.");
    }

    [Fact]
    public void CreateNewVaultCertificate_Success()
    {
        // Arrange
        const ulong vaultId = 1;
        Address owner = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 amount = 2;
        const ulong vestedBlock = 3;
        const ulong createdBlock = 4;

        //Act
        var certificate = new VaultCertificate(vaultId, owner, amount, vestedBlock, createdBlock);

        // Assert
        certificate.Id.Should().Be(0ul);
        certificate.VaultId.Should().Be(vaultId);
        certificate.Owner.Should().Be(owner);
        certificate.Amount.Should().Be(amount);
        certificate.VestedBlock.Should().Be(vestedBlock);
        certificate.Redeemed.Should().Be(false);
        certificate.Revoked.Should().Be(false);
        certificate.CreatedBlock.Should().Be(createdBlock);
        certificate.ModifiedBlock.Should().Be(createdBlock);
    }

    [Fact]
    public void CreateVaultCertificate_Success()
    {
        // Arrange
        const ulong id = 99;
        const ulong vaultId = 1;
        Address owner = "PARwm9EjmivLgqqH1Mroh6zXXNMUiT1GLs";
        UInt256 amount = 2;
        const ulong vestedBlock = 3;
        const ulong createdBlock = 4;
        const ulong modifiedBlock = 5;
        const bool revoked = true;
        const bool redeemed = true;

        //Act
        var certificate = new VaultCertificate(id, vaultId, owner, amount, vestedBlock, redeemed, revoked, createdBlock, modifiedBlock);

        // Assert
        certificate.Id.Should().Be(id);
        certificate.VaultId.Should().Be(vaultId);
        certificate.Owner.Should().Be(owner);
        certificate.Amount.Should().Be(amount);
        certificate.VestedBlock.Should().Be(vestedBlock);
        certificate.Redeemed.Should().Be(redeemed);
        certificate.Revoked.Should().Be(revoked);
    }
}
