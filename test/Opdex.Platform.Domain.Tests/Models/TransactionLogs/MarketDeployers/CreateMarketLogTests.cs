using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.MarketDeployers
{
    public class CreateMarketLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateMarketLog_MarketAddressNotSet_ThrowArgumentNullException(string market)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.market = market;
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
            txLog.router = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.authPoolCreators = true;
            txLog.authProviders = true;
            txLog.authTraders = true;
            txLog.transactionFee = 3u;
            txLog.stakingToken = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.marketFeeEnabled = true;

            // Act
            void Act() => new CreateMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateMarketLog_OwnerAddressNotSet_ThrowArgumentNullException(string owner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.market = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
            txLog.router = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.owner = owner;
            txLog.authPoolCreators = true;
            txLog.authProviders = true;
            txLog.authTraders = true;
            txLog.transactionFee = 3u;
            txLog.stakingToken = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.marketFeeEnabled = true;

            // Act
            void Act() => new CreateMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateMarketLog_RouterAddressNotSet_ThrowArgumentNullException(string router)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.market = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.stakingToken = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.authPoolCreators = true;
            txLog.authProviders = true;
            txLog.authTraders = true;
            txLog.transactionFee = 3u;
            txLog.router = router;
            txLog.marketFeeEnabled = true;

            // Act
            void Act() => new CreateMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateMarketLog_FeeGreaterThanTen_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.market = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.router = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.authPoolCreators = true;
            txLog.authProviders = true;
            txLog.authTraders = true;
            txLog.transactionFee = 11u;
            txLog.stakingToken = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.marketFeeEnabled = true;

            // Act
            void Act() => new CreateMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateMarketLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.market = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj6";
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.router = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.authPoolCreators = true;
            txLog.authProviders = true;
            txLog.authTraders = false;
            txLog.transactionFee = 10u;
            txLog.stakingToken = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj8";
            txLog.marketFeeEnabled = true;

            // Act
            var log = new CreateMarketLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Market.Should().Be(txLog.market);
            log.Owner.Should().Be(txLog.owner);
            log.Router.Should().Be(txLog.router);
            log.AuthPoolCreators.Should().Be(txLog.authPoolCreators);
            log.AuthProviders.Should().Be(txLog.authProviders);
            log.AuthTraders.Should().Be(txLog.authTraders);
            log.TransactionFee.Should().Be(txLog.transactionFee);
            log.EnableMarketFee.Should().Be(txLog.marketFeeEnabled);
            log.StakingToken.Should().Be(txLog.stakingToken);
        }
    }
}