using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Opdex.Platform.Common.Tests.Models.UInt
{
    /// <summary>
    /// Borrowed and referencing https://github.com/stratisproject/Stratis.SmartContracts/blob/master/Stratis.SmartContracts.Tests/UInt128Tests.cs
    /// </summary>
    public class UInt128Tests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UInt128Tests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Create_Zero_UInt128()
        {
            // Act
            var test = new UInt128();

            // Assert
            test.Should().Be(UInt128.Zero);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateFails_UInt128(string amount)
        {
            // Act
            void Act() => new UInt128(amount);

            // Assert
            Assert.Throws<FormatException>(Act);
        }

        [Fact]
        public void Creates_UInt128()
        {
            var test = new UInt128("8765456789098756456789098765");
            var test2 = new UInt128("8765456789098756456789098766");

            (test > test2).Should().BeFalse();
            (test2 > test).Should().BeTrue();

            _testOutputHelper.WriteLine(test.ToString());
        }

        [Fact]
        public void CanConvertToFromUlong()
        {
            UInt128 x = 50;
            Assert.Equal((ulong)50, (ulong)x);
        }

        [Fact]
        public void UInt128FromTooLargeUlongThrowsError()
        {
            UInt128 x = UInt128.Parse("0x10000000000000000");
            Assert.Throws<OverflowException>(() => (ulong)x);
        }

        [Fact]
        public void AddingThrowsOverflowIfResultTooBig()
        {
            UInt128 v1 = UInt128.Parse("0xffffffffffffffffffffffffffffffff");
            UInt128 v2 = UInt128.Parse("0x1");
            Assert.Throws<OverflowException>(() => v1 + v2);
        }

        [Fact]
        public void SubtractingThrowsErrorIfResultIsNegative()
        {
            UInt128 v1 = UInt128.Parse("0xffffffffffffffffffffffffffffffff");
            Assert.Throws<OverflowException>(() => UInt128.Zero - v1);
        }

        [Fact]
        public void MultiplyingThrowsErrorIfResultTooBig()
        {
            UInt128 v1 = UInt128.Parse("0x10000000000000000");
            UInt128 v2 = UInt128.Parse("0x10000000000000000");
            Assert.Throws<OverflowException>(() => v1 * v2);
        }

        [Fact]
        public void DividingByZeroThrowsError()
        {
            UInt128 v1 = UInt128.Parse("0x10000000000000000");
            UInt128 v2 = UInt128.Zero;
            Assert.Throws<DivideByZeroException>(() => v1 / v2);
        }

        [Fact]
        public void ModByZeroThrowsError()
        {
            UInt128 v1 = UInt128.Parse("0x10000000000000000");
            UInt128 v2 = UInt128.Zero;
            Assert.Throws<DivideByZeroException>(() => v1 % v2);
        }

        [Fact]
        public void CanConvertToFromBytes()
        {
            UInt128 x = UInt128.Parse("0xffffffffffffffffffffffffffffffff");
            Assert.Equal(x, new UInt128(x.ToBytes()));
        }

        [Fact]
        public void CanAdd()
        {
            UInt128 v1 = new UInt128("0x325");
            UInt128 v2 = new UInt128("0xf0f");
            UInt128 v3 = v1 + v2;

            Assert.Equal(new UInt128("0x1234"), v3);
        }

        [Fact]
        public void CanSubtract()
        {
            UInt128 v1 = new UInt128("0x1234");
            UInt128 v2 = new UInt128("0x325");
            UInt128 v3 = v1 - v2;

            Assert.Equal(new UInt128("0xf0f"), v3);
        }

        [Fact]
        public void CanMultiply()
        {
            UInt128 v1 = new UInt128("0x1234");
            UInt128 v2 = new UInt128("0x5678");
            UInt128 v3 = v1 * v2;

            Assert.Equal(new UInt128("0x6260060"), v3);
        }

        [Fact]
        public void CanDivide()
        {
            UInt128 v1 = new UInt128("0x6260060");
            UInt128 v2 = new UInt128("0x5678");
            UInt128 v3 = v1 / v2;

            Assert.Equal(new UInt128("0x1234"), v3);
        }

        [Fact]
        public void CanParseLargeNumbers()
        {
            UInt128 v1 = UInt128.Parse("0xffffffffffffffffbfd25e8cd0364141");
            UInt128 v2 = UInt128.Parse("0xbfd25e8cd0364141");
            UInt128 v3 = v1 - v2;

            Assert.Equal(new UInt128("0xffffffffffffffff0000000000000000"), v3);
        }

        [Fact]
        public void CanCast()
        {
            UInt128 v2 = UInt128.Parse("0x12345678");

            Assert.Equal<uint>(0x12345678, (uint)v2);
            Assert.Equal<int>(0x12345678, (int)v2);
            Assert.Equal<ulong>(0x12345678, (ulong)v2);
            Assert.Equal<long>(0x12345678, (long)v2);
            Assert.Equal<UInt128>(0x12345678, v2);
            Assert.Equal<UInt256>(0x12345678, v2);
        }

        [Fact]
        public void CastTooBigNumberCausesOverflow()
        {
            Assert.Throws<OverflowException>(() => (int)UInt128.Parse("0x12345678123456789"));
        }
    }
}
