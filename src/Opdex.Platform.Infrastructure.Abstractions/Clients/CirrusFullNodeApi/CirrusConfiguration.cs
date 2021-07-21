using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi
{
    public class CirrusConfiguration : IValidatable
    {
        public string ApiUrl { get; set; }
        public int ApiPort { get; set; }

        public void Validate()
        {
            // if (!ApiUrl.HasValue())
            // {
            //     throw new Exception($"{nameof(CirrusConfiguration)}.{nameof(ApiUrl)} must not be null or empty.");
            // }
            //
            // if (ApiPort < 1)
            // {
            //     throw new Exception($"{nameof(CirrusConfiguration)}.{nameof(ApiPort)} must be greater than 0.");
            // }
        }
    }
}
