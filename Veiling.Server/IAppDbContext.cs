using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Veiling.Server.Models;

public interface IAppDbContext {
    public DbSet<Bedrijf> Bedrijven { get; set; }
    public DbSet<Locatie> Locaties { get; set; }
    public DbSet<Gebruiker> Gebruikers { get; set; }
    public DbSet<Veilingmeester> Veilingmeesters { get; set; }
    public DbSet<Leverancier> Leveranciers { get; set; }
    public DbSet<Veiling.Server.Models.Veiling> Veilingen { get; set; }
    public DbSet<Kavel> Kavels { get; set; }
    public DbSet<Bod> Boden { get; set; }

    public Task SaveChangesAsync();
    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
