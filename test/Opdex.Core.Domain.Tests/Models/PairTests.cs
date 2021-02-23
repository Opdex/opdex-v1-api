using FluentAssertions;
using Opdex.Core.Domain.Models;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models
{
    public class PairTests
    {
        [Fact]
        public void CreatePair_Success()
        {
            const long id = 1;
            const string address = "Address";
            const long tokenId = 2;
            const decimal reserveCrs = 1.23m;
            const decimal reserveSrc = 124m;

            var pair = new Pair(id, address, tokenId, reserveCrs, reserveSrc);

            pair.Id.Should().Be(id);
            pair.Address.Should().Be(address);
            pair.TokenId.Should().Be(tokenId);
            pair.ReserveCrs.Should().Be(reserveCrs);
            pair.ReserveToken.Should().Be(reserveSrc);
        }
    }
}