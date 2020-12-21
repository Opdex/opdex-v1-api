using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.WebApi.Models;


namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("pairs")]
    public class PairsController : ControllerBase
    {
        public PairsController()
        {
        }
        
        [HttpGet]
        public async Task<ActionResult<List<PairResponseModel>>> Get(CancellationToken cancellationToken)
        {
            var response = Task.FromResult(new List<string> {"MEDI-CRS"});
            
            return Ok(response);
        }
        
        [HttpGet("{pair}")]
        public async Task<ActionResult<PairResponseModel>> Get(string pair, CancellationToken cancellationToken)
        {
            var response = Task.FromResult("Pair");
            
            return Ok(response);
        }
    }
}