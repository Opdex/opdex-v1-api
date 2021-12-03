using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class TransactionsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IApplicationContext> _applicationContextMock;
    private readonly TransactionsController _controller;

    public TransactionsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _applicationContextMock = new Mock<IApplicationContext>();

        _controller = new TransactionsController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
    }


    [Fact]
    public async Task GetTransactions_GetTransactionsWithFilterQuery_Send()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetTransactions(new TransactionFilterParameters(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetTransactionsWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetTransactions_Result_ReturnOk()
    {
        // Arrange
        var vaults = new TransactionsResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetTransactionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionsDto());
        _mapperMock.Setup(callTo => callTo.Map<TransactionsResponseModel>(It.IsAny<TransactionsDto>())).Returns(vaults);

        // Act
        var response = await _controller.GetTransactions(new TransactionFilterParameters(), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(vaults);
    }
}