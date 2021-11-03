using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketTokensControllerTests
{
    public class SwapAmountInTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MarketTokensController _controller;

        public SwapAmountInTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new MarketTokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task SwapAmountIn_GetLiquidityPoolSwapAmountInQuery_Send()
        {
            // Arrange
            Address market = new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH");
            Address tokenIn = new Address("tNgQhNxvachxKGvRonk2S8nrpYi44carYv");
            var request = new SwapAmountInQuoteRequestModel
            {
                TokenOut = new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"),
                TokenOutAmount = FixedDecimal.Parse("10.50000000")
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.SwapAmountIn(market, tokenIn, request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetSwapAmountInQuery>(query => query.Market == market
                                                                                                          && query.TokenIn == tokenIn
                                                                                                          && query.TokenOut == request.TokenOut
                                                                                                          && query.TokenOutAmount == request.TokenOutAmount), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SwapAmountIn_Response_ReturnOk()
        {
            // Arrange
            Address market = new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH");
            Address tokenIn = new Address("tNgQhNxvachxKGvRonk2S8nrpYi44carYv");
            var request = new SwapAmountInQuoteRequestModel
            {
                TokenOut = new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"),
                TokenOutAmount = FixedDecimal.Parse("10.50000000")
            };

            FixedDecimal amountIn = FixedDecimal.Parse("2.55558888");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetSwapAmountInQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(amountIn);

            // Act
            var response = await _controller.SwapAmountIn(market, tokenIn, request, CancellationToken.None);

            // Assert
            response.Result.Should().BeOfType<OkObjectResult>();
            ((SwapAmountInQuoteResponseModel)((OkObjectResult)response.Result).Value).AmountIn.Should().Be(amountIn);
        }
    }
}
