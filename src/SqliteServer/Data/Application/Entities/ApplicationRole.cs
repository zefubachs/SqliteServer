using Microsoft.AspNetCore.Identity;

namespace SqliteServer.Data.Application.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}
