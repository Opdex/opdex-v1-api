using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Admins;

public class AdminEntity
{
    public ulong Id { get; set; }
    public Address Address { get; set; }
}