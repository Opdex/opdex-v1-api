using Opdex.Platform.Common.Models;
using System.Collections;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Tests.Validation;

/// <summary>
/// Data for invalid CRS amounts.
/// </summary>
public class InvalidCRSAmountData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { null };
        yield return new object[] { FixedDecimal.Parse("1") }; // sats value
        yield return new object[] { FixedDecimal.Parse("1.0000000") }; // 7 decimals
        yield return new object[] { FixedDecimal.Parse("1.000000000") }; // 9 decimals
        yield return new object[] { FixedDecimal.Parse("184467440737.09551616") }; // ulong.MaxValue + 1
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Data for a 0 CRS amount.
/// </summary>
public class ZeroCRSAmountData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { FixedDecimal.Parse("0.00000000") };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Data for a 500 CRS amount.
/// </summary>
public class FiveHundredCRSAmountData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { FixedDecimal.Parse("500.00000000") };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Data for a 0 SRC amount.
/// </summary>
public class ZeroSRCAmountData : ZeroCRSAmountData { }

/// <summary>
/// Data for invalid SRC amounts.
/// </summary>
public class InvalidSRCAmountData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { null };
        yield return new object[] { FixedDecimal.Parse("1.0000000000000000000") }; // 19 decimals
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Data for a 0 OLPT amount.
/// </summary>
public class ZeroOLPTAmountData : ZeroCRSAmountData { }

/// <summary>
/// Data for invalid OLPT amounts.
/// </summary>
public class InvalidOLPTAmountData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { null };
        yield return new object[] { FixedDecimal.Parse("1") }; // sats value
        yield return new object[] { FixedDecimal.Parse("1.0000000") }; // 7 decimals
        yield return new object[] { FixedDecimal.Parse("1.000000000") }; // 9 decimals
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
