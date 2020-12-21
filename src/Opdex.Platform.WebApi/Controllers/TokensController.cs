using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("tokens")]
    public class TokensController : ControllerBase
    {
        public TokensController(ILogger<TokensController> logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<TokenResponseModel>>> GetTokens(CancellationToken cancellationToken)
        {
            var response = Task.FromResult(new List<string> {"Cirrus"});
            
            return Ok(response);
        }
        
        [HttpGet("{token}")]
        public async Task<ActionResult<TokenResponseModel>> GetToken(string token, CancellationToken cancellationToken)
        {
            var response = Task.FromResult("Cirrus");
            
            return Ok(response);
        }
    }
}