using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class GetVaultCertificatesWithFilterQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly GetVaultCertificatesWithFilterQueryHandler _handler;

        public GetVaultCertificatesWithFilterQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new GetVaultCertificatesWithFilterQueryHandler(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveVaultByAddressQuery_Send()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByAddressQuery>(query => query.Vault == request.Vault
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveVaultCertificatesWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultCertificatesWithFilterQuery>(query => query.VaultId == vault.Id
                                                                                                             && query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_CertificatesRetrieved_MapResults()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[0];
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(certificates), Times.Once);
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(5, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Certificates.Count().Should().Be(certificates.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(5, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(15, 10, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.Is<IEnumerable<VaultCertificate>>(
                input => !input.Any(certificate => certificate == certificates.First())
            )));
            dto.Certificates.Count().Should().Be(certificates.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(5, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(15, 10, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.Is<IEnumerable<VaultCertificate>>(
                input => !input.Any(certificate => certificate == certificates.Last())
            )));
            dto.Certificates.Count().Should().Be(certificates.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(5, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(15, 10, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, certificates[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(55, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(60, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(65, 10, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, certificates[^2].Id);
            AssertPrevious(dto.Cursor, certificates[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(55, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(60, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(65, 10, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, certificates[^1].Id);
            AssertPrevious(dto.Cursor, certificates[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(55, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(60, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, certificates[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetVaultCertificatesWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var vault = new Vault(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", 15, UInt256.Parse("10000000000"), 500, 505);
            var certificates = new VaultCertificate[]
            {
                new VaultCertificate(55, 10, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", UInt256.Parse("1000000000"), 10500, false, false, 500, 505),
                new VaultCertificate(60, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("1000000000"), 10500, false, false, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultCertificatesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificates);
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<VaultCertificateDto>>(It.IsAny<IEnumerable<VaultCertificate>>()))
                       .Returns<IEnumerable<VaultCertificate>>(input => new VaultCertificateDto[input.Count()]);

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, certificates[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, long pointer)
        {
            VaultCertificatesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, long pointer)
        {
            VaultCertificatesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
