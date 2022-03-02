using FluentAssertions;
using Moq;
using Opdex.Platform.Application.Cache;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Cache;

public class WrappedTokenTrustValidatorTests
{
    private readonly Mock<ISupportedContractsModule> _supportedContractsModuleMock;
    private readonly WrappedTokenTrustValidator _wrappedTokenTrustValidator;

    public WrappedTokenTrustValidatorTests()
    {
        _supportedContractsModuleMock = new Mock<ISupportedContractsModule>();
        _wrappedTokenTrustValidator = new WrappedTokenTrustValidator(_supportedContractsModuleMock.Object);
    }

    [Fact]
    public async Task Validate_OnFirstCall_SupportedContractsModuleGetList()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _wrappedTokenTrustValidator.Validate(Address.Empty, cancellationToken);

        // Assert
        _supportedContractsModuleMock.Verify(callTo => callTo.GetList(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Validate_OnSecondCall_DoNotCallOutToNode()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        await _wrappedTokenTrustValidator.Validate(Address.Empty, cancellationToken);

        // Act
        await _wrappedTokenTrustValidator.Validate(Address.Empty, cancellationToken);

        // Assert
        _supportedContractsModuleMock.Verify(callTo => callTo.GetList(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Validate_NotTrusted_ReturnFalse()
    {
        // Arrange
        var src20Address = new Address[]
        {
            new("tNVR1r6WSWSCK7XVQsz9aJk3CdBGGvFgY5"),
            new("tWD9SN1hPcbefQAWarkB7Jg4xjkDkBYjvH"),
            new("tQhh41Z6LCbiJEF78c2myQHqvbptsfPmSu")
        };

        _supportedContractsModuleMock.Setup(callTo => callTo.GetList(It.IsAny<CancellationToken>()))
            .ReturnsAsync(src20Address.Select(address => new InterfluxMappingDto
            {
                NativeNetwork = NativeChainType.Ethereum,
                NativeAddress = "NATIVE ADDRESS",
                Src20Address = address,
                TokenName = "TOKEN NAME"
            }));


        // Act
        var isValid = await _wrappedTokenTrustValidator.Validate(Address.Empty);

        // Assert
        isValid.Should().Be(false);
    }

    [Fact]
    public async Task Validate_Trusted_ReturnTrue()
    {
        // Arrange
        var src20Address = new Address[]
        {
            new("tNVR1r6WSWSCK7XVQsz9aJk3CdBGGvFgY5"),
            new("tWD9SN1hPcbefQAWarkB7Jg4xjkDkBYjvH"),
            new("tQhh41Z6LCbiJEF78c2myQHqvbptsfPmSu")
        };

        _supportedContractsModuleMock.Setup(callTo => callTo.GetList(It.IsAny<CancellationToken>()))
            .ReturnsAsync(src20Address.Select(address => new InterfluxMappingDto
            {
                NativeNetwork = NativeChainType.Ethereum,
                NativeAddress = "NATIVE ADDRESS",
                Src20Address = address,
                TokenName = "TOKEN NAME"
            }));


        // Act
        var isValid = await _wrappedTokenTrustValidator.Validate(new Address("tNVR1r6WSWSCK7XVQsz9aJk3CdBGGvFgY5"));

        // Assert
        isValid.Should().Be(true);
    }
}
