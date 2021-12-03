using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Opdex.Platform.WebApi.Conventions;

public class NetworkActionHidingConventionMvcOptions : IConfigureOptions<MvcOptions>
{
    private readonly NetworkActionHidingConvention _convention;

    public NetworkActionHidingConventionMvcOptions(NetworkActionHidingConvention convention)
    {
        _convention = convention;
    }

    public void Configure(MvcOptions options)
    {
        options.Conventions.Add(_convention);
    }
}