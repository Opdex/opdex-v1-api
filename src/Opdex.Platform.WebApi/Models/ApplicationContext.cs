using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Models;
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

        public Address Market => GetMarket();

        public Address Wallet => GetWallet();

        private Address GetMarket()
        {
            var subject = _httpContextAccessor.HttpContext
                .User.Claims.FirstOrDefault(claim => claim.Type == "market");

            return new Address(subject?.Value);
        }

        private Address GetWallet()
        {
            var subject = _httpContextAccessor.HttpContext
                .User.Claims.FirstOrDefault(claim => claim.Type == "wallet");

            return new Address(subject?.Value);
        }
    }
}
