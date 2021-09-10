using Newtonsoft.Json;
using Opdex.Platform.Common;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("Opdex.Platform.Application.Tests.SerializationSetup", "Opdex.Platform.Application.Tests")]

namespace Opdex.Platform.Application.Tests
{
    public class SerializationSetup : XunitTestFramework
    {
        public SerializationSetup(IMessageSink messageSink) : base(messageSink)
        {
            JsonConvert.DefaultSettings = () => Serialization.DefaultJsonSettings;
        }
    }
}
