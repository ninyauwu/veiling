using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server
{
    public class AppDbContext : IdentityDbContext<Gebruiker>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<Locatie> Locaties { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Veilingmeester> Veilingmeesters { get; set; }
        public DbSet<Leverancier> Leveranciers { get; set; }
        public DbSet<Models.Veiling> Veilingen { get; set; }
        public DbSet<Kavel> Kavels { get; set; }
        public DbSet<KavelVeiling> KavelVeilingen { get; set; }
        public DbSet<Bod> Boden { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasDefaultSchema("identity");

            modelBuilder.Entity<Bedrijf>()
                .HasKey(b => b.Bedrijfscode);

            modelBuilder.Entity<Bedrijf>()
                .HasMany(b => b.Gebruikers)
                .WithOne(g => g.Bedrijf)
                .HasForeignKey(g => g.BedrijfId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Gebruiker>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Gebruiker>()
                .HasMany(g => g.Boden)
                .WithOne(b => b.Gebruiker)
                .HasForeignKey(b => b.GebruikerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Veilingmeester>()
                .HasKey(vm => vm.Id);

            modelBuilder.Entity<Veilingmeester>()
                .HasOne(vm => vm.Gebruiker)
                .WithMany()
                .HasForeignKey(vm => vm.GebruikerId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Leverancier>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Leverancier>()
                .HasOne(l => l.Bedrijf)
                .WithMany()
                .HasForeignKey(l => l.BedrijfId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Leverancier>()
                .HasMany(l => l.Kavels)
                .WithOne(k => k.Leverancier)
                .HasForeignKey(k => k.LeverancierId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Models.Veiling>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Models.Veiling>()
                .HasMany(v => v.Kavels)
                .WithOne(k => k.Veiling)
                .HasForeignKey(k => k.VeilingId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Kavel>()
                .HasKey(k => k.Id);

            modelBuilder.Entity<Kavel>()
                .HasMany(k => k.Boden)
                .WithOne(b => b.Kavel)
                .HasForeignKey(b => b.KavelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Kavel>()
                .HasMany(k => k.KavelVeilingen)
                .WithOne(k => k.Kavel);

            modelBuilder.Entity<KavelVeiling>()
                .HasKey(kv => kv.Id);

            modelBuilder.Entity<KavelVeiling>()
                .HasOne(kv => kv.Kavel)
                .WithMany(k => k.KavelVeilingen)
                .HasForeignKey(kv => kv.KavelId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Bod>()
                .HasKey(b => b.Id);
            
            modelBuilder.Entity<Locatie>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Locatie>()
                .HasMany(l => l.Veilingen)
                .WithOne(v => v.Locatie)
                .HasForeignKey(v => v.LocatieId)
                .OnDelete(DeleteBehavior.SetNull);
            
        }

        public async Task SaveChangesAsync() {
            await base.SaveChangesAsync();
        }
    }
}
