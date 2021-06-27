using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class WalletControllerTests
    {
        private readonly Mock<IApplicationContext> _applicationContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _applicationContextMock = new Mock<IApplicationContext>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _controller = new WalletController(_applicationContextMock.Object, _mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_Send()
        {
            // Arrange
            var token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var cancellationToken = new CancellationTokenSource().Token;

            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns(owner);

            // Act
            await _controller.GetApprovedAllowanceForToken(spender, token, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetAddressAllowanceForTokenQuery>(
                query => query.Token == token
                      && query.Owner == owner
                      && query.Spender == spender
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_ReturnMapped()
        {
            // Arrange
            var addressAllowanceDto = new AddressAllowanceDto();
            var approvedAllowanceResponseModel = new ApprovedAllowanceResponseModel { Amount = "500000" };

            _applicationContextMock.Setup(callTo => callTo.Wallet).Returns("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressAllowanceForTokenQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowanceDto);
            _mapperMock.Setup(callTo => callTo.Map<ApprovedAllowanceResponseModel>(addressAllowanceDto)).Returns(approvedAllowanceResponseModel);

            // Act
            var response = await _controller.GetApprovedAllowanceForToken("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                                                                          "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                          CancellationToken.None);

            // Assert
            response.Value.Should().Be(approvedAllowanceResponseModel);
        }
    }
}
