using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Opdex.Platform.Application.Exceptions;
using System;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Middleware
{
    /// <summary>
    /// Provides middleware for returning redirect responses based on exceptions thrown.
    /// </summary>
    public class RedirectToResourceMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectToResourceMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (TokenAlreadyIndexedException e)
            {
                httpContext.Response.Headers.Add(HeaderNames.Location, $"/tokens/{e.Token}");
                httpContext.Response.StatusCode = StatusCodes.Status303SeeOther;
                await httpContext.Response.CompleteAsync();
            }
        }
    }
}
