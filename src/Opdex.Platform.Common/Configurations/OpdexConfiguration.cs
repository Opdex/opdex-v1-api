using Opdex.Platform.Common.Enums;
using System;

namespace Opdex.Platform.Common.Configurations
{
    public class OpdexConfiguration : IValidatable
    {
        public string ConnectionString { get; set; }
        public NetworkType Network { get; set; }

        public string InstanceId { get; } = Guid.NewGuid().ToString();

        public void Validate()
        {
            // if (!ConnectionString.HasValue())
            // {
            //     throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(ConnectionString)} must not be null or empty.");
            // }
            //
            // if (!Network.IsValid())
            // {
            //     throw new Exception($"{nameof(OpdexConfiguration)}.{nameof(Network)} must be a valid network type.");
            // }
        }
    }
}
