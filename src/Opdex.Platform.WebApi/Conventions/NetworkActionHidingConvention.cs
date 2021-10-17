using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Middleware;
using System.Linq;

namespace Opdex.Platform.WebApi.Conventions
{
    /// <summary>
    /// A convention to hide actions decorated with <see cref="NetworkAttribute" />, that are not configured to the network which the application is configured.
    /// </summary>
    public class NetworkActionHidingConvention : IApplicationModelConvention
    {
        public NetworkActionHidingConvention(OpdexConfiguration opdexConfiguration)
        {
            ConfiguredNetwork = opdexConfiguration.Network;
        }

        public NetworkType ConfiguredNetwork { get; }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    if (!(action.Attributes.SingleOrDefault(attribute => attribute is NetworkAttribute) is NetworkAttribute networkFilter)) continue;
                    if (networkFilter.Network == ConfiguredNetwork) continue;

                    action.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}
