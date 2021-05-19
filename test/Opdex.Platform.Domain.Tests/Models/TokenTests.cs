using FluentAssertions;
using Opdex.Platform.Domain.Models.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class TokenTests
    {
        [Fact]
        public void CreatesToken_Success()
        {
            const string address = "Address";
            const string name = "Opdex Token";
            const string symbol = "OPDX";
            const int decimals = 18;
            const long sats = 10000000000000000;
            const string totalSupply = "987654321";
            const ulong createdBlock = 3;
            const ulong modifiedBlock = 4;

            var token = new Token(address, name, symbol, decimals, sats, totalSupply, createdBlock, modifiedBlock);

            token.Id.Should().Be(0);
            token.Address.Should().Be(address);
            token.Name.Should().Be(name);
            token.Symbol.Should().Be(symbol);
            token.Decimals.Should().Be(decimals);
            token.Sats.Should().Be(sats);
            token.TotalSupply.Should().Be(totalSupply);
        }
    }
}