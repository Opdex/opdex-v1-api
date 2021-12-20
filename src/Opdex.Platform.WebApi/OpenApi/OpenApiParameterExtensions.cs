using NSwag;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.OpenApi;

public static class OpenApiParameterExtensions
{
    public static void DefineAsNetworkAddress(this OpenApiParameter parameter, Address example)
    {
        parameter.Schema.Example = example.ToString();
        parameter.Schema.MinLength = 30;
        parameter.Schema.MaxLength = 42;
    }

    public static void DefineAsNetworkAddressOrCrs(this OpenApiParameter parameter, Address example)
    {
        parameter.Schema.Example = example.ToString();
        parameter.Schema.MinLength = 3;
        parameter.Schema.MaxLength = 42;
    }

    public static void DefineAsSha256(this OpenApiParameter parameter, Sha256 example)
    {
        parameter.Schema.Example = example.ToString();
        parameter.Schema.Pattern = @"^[A-Fa-f0-9]{64}$";
        parameter.Schema.MinLength = 64;
        parameter.Schema.MaxLength = 64;
    }
}
