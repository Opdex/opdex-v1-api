using System.Collections;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests;

/// <summary>
/// Data for a null, empty or whitespace <see cref="string" />
/// </summary>
public class NullOrWhitespaceStringData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { null };
        yield return new object[] { "" };
        yield return new object[] { "   " };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}