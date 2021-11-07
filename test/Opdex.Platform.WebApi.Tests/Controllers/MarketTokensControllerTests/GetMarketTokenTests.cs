using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketTokensControllerTests
{
    public class GetMarketTokenTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MarketTokensController _controller;

        public GetMarketTokenTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new MarketTokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task GetMarketToken_GetTokenByAddressQuery_Send()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetMarketToken(market, token, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMarketTokenByMarketAndTokenAddressQuery>(query => query.Market == market &&
                                                                                                                  query.Token == token),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetMarketToken_MarketTokenDto_Map()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var dto = new MarketTokenDto();

            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketTokenByMarketAndTokenAddressQuery>(), cancellationToken)).ReturnsAsync(dto);

            // Act
            await _controller.GetMarketToken(market, token, cancellationToken);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<MarketTokenResponseModel>(It.IsAny<MarketTokenDto>()), Times.Once);
        }

        [Fact]
        public async Task GetMarketToken_ReturnOk()
        {
            // Arrange
            Address market = "fJ6u1yzNPDw7uPGZPZpB4iW4LHVEPMKehX";
            Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            var dto = new MarketTokenDto();
            var tokenResponse = new MarketTokenResponseModel();

            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketTokenByMarketAndTokenAddressQuery>(), cancellationToken)).ReturnsAsync(dto);
            _mapperMock.Setup(callTo => callTo.Map<MarketTokenResponseModel>(It.IsAny<MarketTokenDto>())).Returns(tokenResponse);

            // Act
            var response = await _controller.GetMarketToken(market, token, cancellationToken);

            // Act
            response.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response.Result).Value.Should().Be(tokenResponse);
        }
    }
}
