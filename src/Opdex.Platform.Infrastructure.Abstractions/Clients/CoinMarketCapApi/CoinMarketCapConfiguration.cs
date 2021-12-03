using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi;

public class CoinMarketCapConfiguration : IValidatable
{
    public string ApiUrl { get; set; }
    public string ApiKey { get; set; }
    public string HeaderName { get; set; }

    public void Validate()
    {
        if (!ApiUrl.HasValue())
        {
            throw new Exception($"{nameof(CoinMarketCapConfiguration)}.{nameof(ApiUrl)} must not be null or empty.");
        }

        if (!ApiKey.HasValue())
        {
            throw new Exception($"{nameof(CoinMarketCapConfiguration)}.{nameof(ApiKey)} must not be null or empty.");
        }

        if (!HeaderName.HasValue())
        {
            throw new Exception($"{nameof(CoinMarketCapConfiguration)}.{nameof(HeaderName)} must not be null or empty.");
        }
    }
}