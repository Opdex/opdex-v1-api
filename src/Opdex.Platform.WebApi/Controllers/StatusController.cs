using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Models.Responses.Status;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("status")]
    public class StatusController : ControllerBase
    {
        private readonly OpdexConfiguration _opdexConfiguration;

        public StatusController(OpdexConfiguration opdexConfiguration)
        {
            _opdexConfiguration = opdexConfiguration;
        }

        /// <summary>
        /// Get Status
        /// </summary>
        /// <remarks>Includes status details for the running instance of the application.</remarks>
        /// <returns>Status details.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(StatusResponseModel), StatusCodes.Status200OK)]
        public ActionResult<StatusResponseModel> GetStatus()
        {
            return new StatusResponseModel
            {
                Commit = _opdexConfiguration.CommitHash,
                Identifier = _opdexConfiguration.InstanceId
            };
        }
    }
}