using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Tokens;

public class CallCirrusTrustedWrappedTokenQueryHandlerTests
{
    private readonly Mock<ISupportedContractsModule> _supportedContractsModuleMock;
    private readonly CallCirrusTrustedWrappedTokenQueryHandler _handler;

    public CallCirrusTrustedWrappedTokenQueryHandlerTests()
    {
        _supportedContractsModuleMock = new Mock<ISupportedContractsModule>();
        _handler = new CallCirrusTrustedWrappedTokenQueryHandler(_supportedContractsModuleMock.Object);
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
                NativeChainAddress = "NATIVE ADDRESS",
                Src20Address = address,
                TokenName = "TOKEN NAME"
            }));


        // Act
        var isValid = await _handler.Handle(new CallCirrusTrustedWrappedTokenQuery(new Address("tGVp2oniJ9WDNzRRT9xXTgteKdN1ZjC7mp")), CancellationToken.None);

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
                NativeChainAddress = "NATIVE ADDRESS",
                Src20Address = address,
                TokenName = "TOKEN NAME"
            }));


        // Act
        var isValid = await _handler.Handle(new CallCirrusTrustedWrappedTokenQuery(new Address("tNVR1r6WSWSCK7XVQsz9aJk3CdBGGvFgY5")), CancellationToken.None);

        // Assert
        isValid.Should().Be(true);
    }
}
