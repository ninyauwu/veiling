using Microsoft.EntityFrameworkCore;

namespace Veiling.Server;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }
}
