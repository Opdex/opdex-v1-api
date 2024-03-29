using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Blocks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks;

public class RetrieveLatestBlockQueryHandlerTests
{
    private readonly RetrieveLatestBlockQueryHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public RetrieveLatestBlockQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();
        var logger = new NullLogger<RetrieveLatestBlockQueryHandler>();

        _mediator = new Mock<IMediator>();
        _handler = new RetrieveLatestBlockQueryHandler(_mediator.Object, mapper);
    }

    [Fact]
    public async Task RetrievesLatestBlock_Success()
    {
        const ulong expectedBlockHeight = 1234ul;
        Sha256 expectedBlockHash = new Sha256(52858329043290832);
        var expectedBlockTime = DateTime.UtcNow;
        var expectedBlockMedianTime = DateTime.UtcNow;

        _mediator.Setup(m => m.Send(It.IsAny<SelectLatestBlockQuery>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new Block(expectedBlockHeight, expectedBlockHash, expectedBlockTime, expectedBlockMedianTime)));

        var request = new RetrieveLatestBlockQuery();
        var response = await _handler.Handle(request, CancellationToken.None);

        response.Height.Should().Be(expectedBlockHeight);
        response.Hash.Should().Be(expectedBlockHash);
        response.Time.Should().Be(expectedBlockTime);
        response.MedianTime.Should().Be(expectedBlockMedianTime);
    }
}