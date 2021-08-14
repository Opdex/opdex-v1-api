using FluentAssertions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools
{
    public class LiquidityPoolTests
    {
        [Fact]
        public void CreatePool_Success()
        {
            const string address = "Address";
            const long srcId = 2;
            const long lptId = 4;
            const long marketId = 1;
            const ulong createdBlock = 3;

            var pool = new LiquidityPool(address, srcId, lptId, marketId, createdBlock);

            pool.Id.Should().Be(0);
            pool.Address.Should().Be(address);
            pool.SrcTokenId.Should().Be(srcId);
            pool.LpTokenId.Should().Be(lptId);
            pool.CreatedBlock.Should().Be(createdBlock);
            pool.ModifiedBlock.Should().Be(createdBlock);
        }
    }
}
