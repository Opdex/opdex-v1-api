using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Opdex.Platform.Common.Models;
using System.Reflection;

namespace Opdex.Platform.Common.Converters;

public class OpdexContractResolver : CamelCasePropertyNamesContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyType == typeof(Address)) property.DefaultValueHandling = DefaultValueHandling.Ignore;

        return property;
    }
}
