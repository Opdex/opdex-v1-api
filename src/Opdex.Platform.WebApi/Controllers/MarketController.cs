using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("market")]
    public class MarketController : ControllerBase
    {
        public MarketController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetMarketDetails(CancellationToken cancellationToken)
        {
            return Ok();
        }
    }
}