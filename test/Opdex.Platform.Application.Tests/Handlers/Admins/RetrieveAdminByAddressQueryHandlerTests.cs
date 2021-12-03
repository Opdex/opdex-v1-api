using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Admins;
using Opdex.Platform.Application.Handlers.Admins;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Admins;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Admins;

public class RetrieveAdminByAddressQueryHandlerTests
{
    private readonly RetrieveAdminByAddressQueryHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public RetrieveAdminByAddressQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new RetrieveAdminByAddressQueryHandler(_mediator.Object);
    }

    [Fact]
    public void RetrieveAdminByAddressQuery_InvalidBlock_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        static void Act() => new RetrieveAdminByAddressQuery(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Address must not be empty.");
    }

    [Fact]
    public async Task RetrieveAdminByAddressQuery_Sends_ExecuteRewindToBlockCommand()
    {
        // Arrange
        Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        try
        {
            await _handler.Handle(new RetrieveAdminByAddressQuery(address), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<SelectAdminByAddressQuery>(q => q.Address == address &&
                                                                                     q.FindOrThrow == true), CancellationToken.None));
    }

    [Fact]
    public async Task RetrieveAdminByAddressQuery_Success()
    {
        // Arrange
        var expected = new Admin(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj");
        var command = new RetrieveAdminByAddressQuery(expected.Address);

        _mediator.Setup(m => m.Send(It.IsAny<SelectAdminByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().Be(expected);
    }
}