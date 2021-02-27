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
            const string address = "Address";
            const long tokenId = 2;
            const ulong reserveCrs = 112;
            const string reserveSrc = "1234";

            var pair = new Pair(address, tokenId, reserveCrs, reserveSrc);

            pair.Id.Should().Be(0);
            pair.Address.Should().Be(address);
            pair.TokenId.Should().Be(tokenId);
            pair.ReserveCrs.Should().Be(reserveCrs);
            pair.ReserveSrc.Should().Be(reserveSrc);
        }
    }
}