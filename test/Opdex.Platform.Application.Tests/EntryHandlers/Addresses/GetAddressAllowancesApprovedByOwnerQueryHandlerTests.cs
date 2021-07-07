using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses
{
    public class GetAddressAllowancesApprovedByOwnerQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<AddressAllowance, AddressAllowanceDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetAddressAllowancesApprovedByOwnerQueryHandler _handler;

        public GetAddressAllowancesApprovedByOwnerQueryHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<AddressAllowance, AddressAllowanceDto>>();
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetAddressAllowancesApprovedByOwnerQueryHandler(_assemblerMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task TokenIsSet_RetrieveTokenByAddressQuery_Send()
        {
            // Arrange
            var token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetAddressAllowancesApprovedByOwnerQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl", token);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == token && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task TokenNotSet_RetrieveTokenByAddressQuery_DoNotSend()
        {
            // Arrange
            var token = "";
            var request = new GetAddressAllowancesApprovedByOwnerQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl", token);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task TokenIsSet_RetrieveAddressAllowancesByOwnerWithFilterQuery_Send()
        {
            // Arrange
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXK";
            var tokenAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXL";
            var request = new GetAddressAllowancesApprovedByOwnerQuery(owner, spender, tokenAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, "21000000", 5, 15);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowancesByOwnerWithFilterQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.TokenId == token.Id
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task TokenNotSet_RetrieveAddressAllowancesByOwnerWithFilterQuery_Send()
        {
            // Arrange
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXK";
            var tokenAddress = "";
            var request = new GetAddressAllowancesApprovedByOwnerQuery(owner, spender, tokenAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, "21000000", 5, 15);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowancesByOwnerWithFilterQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.TokenId == 0
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task HappyPath_ForEachAddressAllowance_Assemble()
        {
            // Arrange
            var request = new GetAddressAllowancesApprovedByOwnerQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj");

            var token = new Token(5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, "21000000", 5, 15);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var addressAllowances = new List<AddressAllowance>
            {
                new AddressAllowance(5, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "50000000000", 5, 15),
                new AddressAllowance(6, 12, "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "999999", 6, 15),
                new AddressAllowance(7, 14, "PXJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXK", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "52414433100", 7, 15),
                new AddressAllowance(8, 16, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "35421", 7, 15),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowancesByOwnerWithFilterQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowances);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<AddressAllowance>()), Times.Exactly(addressAllowances.Count));
        }
    }
}
