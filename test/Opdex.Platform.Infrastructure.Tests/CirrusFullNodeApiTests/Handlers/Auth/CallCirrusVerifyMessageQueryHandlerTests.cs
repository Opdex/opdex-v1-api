using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Auth;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Auth;

public class CallCirrusVerifyMessageQueryHandlerTests
{
    private readonly Mock<IWalletModule> _moduleMock;

    private readonly CallCirrusVerifyMessageQueryHandler _handler;

    public CallCirrusVerifyMessageQueryHandlerTests()
    {
        _moduleMock = new Mock<IWalletModule>();
        _handler = new CallCirrusVerifyMessageQueryHandler(_moduleMock.Object);
    }

    [Fact]
    public async Task Handle_VerifyMessage_Call()
    {
        // Arrange
        var request = new CallCirrusVerifyMessageQuery("MESSAGE_CONTENT", new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"), "BASE64_SIGNATURE");
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _moduleMock.Verify(callTo => callTo.VerifyMessage(It.Is<VerifyMessageRequestDto>(dto => dto.Message == request.Message
                                                                                                && dto.Signature == request.Signature
                                                                                                && dto.ExternalAddress == request.Signer), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_VerifyMessage_Return()
    {
        // Arrange
        var request = new CallCirrusVerifyMessageQuery("MESSAGE_CONTENT", new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"), "BASE64_SIGNATURE");

        _moduleMock.Setup(callTo => callTo.VerifyMessage(It.IsAny<VerifyMessageRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(true);
    }
}