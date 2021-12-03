using Newtonsoft.Json;
using Opdex.Platform.Common;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Opdex.Platform.Infrastructure.Tests.SerializationSetup", "Opdex.Platform.Infrastructure.Tests")]

namespace Opdex.Platform.Infrastructure.Tests;

public class SerializationSetup : XunitTestFramework
{
    public SerializationSetup(IMessageSink messageSink) : base(messageSink)
    {
        JsonConvert.DefaultSettings = () => Serialization.DefaultJsonSettings;
    }
}