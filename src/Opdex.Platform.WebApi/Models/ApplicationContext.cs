using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Opdex.Platform.WebApi.Models
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Market => GetMarket();

        public string Wallet => GetWallet();

        private string GetMarket()
        {
            var subject = _httpContextAccessor.HttpContext
                .User.Claims.FirstOrDefault(claim => claim.Type == "market");

            return subject?.Value;
        }

        private string GetWallet()
        {
            var subject = _httpContextAccessor.HttpContext
                .User.Claims.FirstOrDefault(claim => claim.Type == "wallet");

            return subject?.Value;
        }
    }
}
