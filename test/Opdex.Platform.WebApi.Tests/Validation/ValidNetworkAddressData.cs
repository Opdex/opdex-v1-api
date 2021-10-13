using Opdex.Platform.Common.Models;
using System.Collections;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Tests.Validation
{
    public class ValidNetworkAddressData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm") };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
