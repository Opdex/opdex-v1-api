using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class PoolTests
    {
        [Fact]
        public void CreatePool_Success()
        {
            const string address = "Address";
            const long tokenId = 2;
            const ulong reserveCrs = 112;
            const string reserveSrc = "1234";
            const long marketId = 1;

            var pool = new LiquidityPool(address, tokenId, marketId, reserveCrs, reserveSrc);

            pool.Id.Should().Be(0);
            pool.Address.Should().Be(address);
            pool.TokenId.Should().Be(tokenId);
            pool.ReserveCrs.Should().Be(reserveCrs);
            pool.ReserveSrc.Should().Be(reserveSrc);
        }
    }
}