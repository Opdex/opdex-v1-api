namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class LogDto
    {
        public string Address { get; set; }
        public string[] Topics { get; set; }
        public string Data { get; set; }
        public object Log { get; set; }
    }
}