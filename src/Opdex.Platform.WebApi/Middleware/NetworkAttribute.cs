using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Middleware
{
    /// <summary>
    /// An attribute that can be placed on a controller action to enable it only for a specified network type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NetworkAttribute : ActionFilterAttribute
    {
        public NetworkAttribute(NetworkType network)
        {
            Network = network;
        }

        /// <summary>
        /// The network that the action is enabled on.
        /// </summary>
        public NetworkType Network { get; }

        /// <summary>
        /// Performs controller action pre-processing to ensure the configured network matches the specified network type.
        /// If the network does not match, any incoming request will return 404 Not Found.
        /// </summary>
        /// <param name="context">The controller action context.</param>
        /// <param name="next">The action delegate.</param>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetRequiredService<OpdexConfiguration>();

            if (config.Network != Network)
            {
                // leave middleware to handle resolving the response
                throw new NotFoundException();
            }

            await next();
        }
    }
}
