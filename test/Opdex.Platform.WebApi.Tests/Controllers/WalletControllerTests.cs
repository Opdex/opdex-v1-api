using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class WalletControllerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _controller = new WalletController(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_Send()
        {
            // Arrange
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _controller.GetApprovedAllowances(owner, spender, token, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetAddressAllowancesApprovedByOwnerQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.Token == token
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetApprovedAllowanceForToken_GetAddressAllowanceForTokenQuery_ReturnMapped()
        {
            // Arrange
            var addressAllowancesDto = new List<AddressAllowanceDto> { new AddressAllowanceDto() };
            var approvedAllowancesResponseModel = new List<ApprovedAllowanceResponseModel> { new ApprovedAllowanceResponseModel { Allowance = "500000" } };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetAddressAllowancesApprovedByOwnerQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowancesDto);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<ApprovedAllowanceResponseModel>>(addressAllowancesDto)).Returns(approvedAllowancesResponseModel);

            // Act
            var response = await _controller.GetApprovedAllowances("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl", "", "", CancellationToken.None);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response).Value.Should().BeEquivalentTo(approvedAllowancesResponseModel);
        }
    }
}
