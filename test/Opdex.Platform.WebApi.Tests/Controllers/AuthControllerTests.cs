using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Auth;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Auth;
using System;
using System.Threading;
using Xunit;
using static Opdex.Platform.WebApi.Auth.AuthConfiguration;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var configuration = new AuthConfiguration
            {
                StratisOpenAuthProtcol = new StratisOpenAuthConfiguration
                {
                    CallbackBase = "api.opdex.com/auth"
                }
            };

            _controller = new AuthController(configuration, Mock.Of<IMediator>());
        }

        [Fact]
        public void StratisOpenAuthCallback_Expired_ThrowInvalidDataException()
        {
            // Arrange
            var query = new StratisOpenAuthCallbackQuery
            {
                Uid = Guid.NewGuid().ToString(),
                Exp = 1635200000 // 25 Oct 2021
            };
            var body = new StratisOpenAuthCallbackBody
            {
                PublicKey = new Address(""),
                Signature = ""
            };

            // Act
            void Act() => _controller.StratisOpenAuthCallback(query, body, CancellationToken.None);

            // Assert
            var exception = Assert.Throws<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("exp");
        }
    }
}