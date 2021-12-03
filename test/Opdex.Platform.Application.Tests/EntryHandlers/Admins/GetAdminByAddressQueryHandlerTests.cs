using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Application.Abstractions.Queries.Admins;
using Opdex.Platform.Application.EntryHandlers.Admins;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Admins;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Admins;

public class GetAdminByAddressQueryHandlerTests
{
    private readonly GetAdminByAddressQueryHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public GetAdminByAddressQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

        _mediator = new Mock<IMediator>();
        _handler = new GetAdminByAddressQueryHandler(_mediator.Object, mapper);
    }

    [Fact]
    public void GetAdminByAddressQuery_InvalidBlock_ThrowsArgumentNullException()
    {
        // Arrange
        // Act
        static void Act() => new GetAdminByAddressQuery(null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Address must not be empty.");
    }

    [Fact]
    public async Task GetAdminByAddressQuery_Sends_ExecuteRewindToBlockCommand()
    {
        // Arrange
        Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        try
        {
            await _handler.Handle(new GetAdminByAddressQuery(address), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveAdminByAddressQuery>(q => q.Address == address &&
                                                                                       q.FindOrThrow == true), CancellationToken.None));
    }

    [Fact]
    public async Task GetAdminByAddressQuery_Success()
    {
        // Arrange
        var expected = new Admin(1, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj");
        var command = new GetAdminByAddressQuery(expected.Address);

        _mediator.Setup(m => m.Send(It.IsAny<RetrieveAdminByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Id.Should().Be(expected.Id);
        response.Address.Should().Be(expected.Address);
    }
}