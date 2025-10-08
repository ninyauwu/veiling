using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Veilingmeester> Veilingmeesters { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<Leverancier> Leveranciers { get; set; }
        public DbSet<VeilingItem> Veilingen { get; set; }  // 👈 Update!
        public DbSet<Kavel> Kavels { get; set; }
        public DbSet<Bod> Boden { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Veilingmeester>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<Gebruiker>()
                .HasKey(g => g.GebruikerId);

            modelBuilder.Entity<Bedrijf>()
                .HasKey(b => b.Bedrijfscode);

            modelBuilder.Entity<Leverancier>()
                .HasKey(l => l.Id);

            // Veiling -> VeilingItem 👇
            modelBuilder.Entity<VeilingItem>()
                .HasKey(v => v.Id);
            
            modelBuilder.Entity<VeilingItem>()
                .HasOne(v => v.Veilingmeester)
                .WithMany(vm => vm.Veilingen)
                .HasForeignKey(v => v.VeilingmeesterId)
                .OnDelete(DeleteBehavior.SetNull);

            // Kavel
            modelBuilder.Entity<Kavel>()
                .HasKey(k => k.Id);
            
            modelBuilder.Entity<Kavel>()
                .HasOne(k => k.Gebruiker)
                .WithMany(g => g.Kavels)
                .HasForeignKey(k => k.GebruikerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Kavel>()
                .HasOne(k => k.Veiling)
                .WithMany(v => v.Kavels)
                .HasForeignKey(k => k.VeilingId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Kavel>()
                .HasOne(k => k.Bedrijf)
                .WithMany(b => b.Kavels)
                .HasForeignKey(k => k.Bedrijfscode)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Kavel>()
                .HasOne(k => k.Leverancier)
                .WithMany(l => l.Kavels)
                .HasForeignKey(k => k.LeverancierId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Bod>()
                .HasKey(b => b.Id);
            
            modelBuilder.Entity<Bod>()
                .HasOne(b => b.Gebruiker)
                .WithMany(g => g.Boden)
                .HasForeignKey(b => b.GebruikerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Bod>()
                .HasOne(b => b.Kavel)
                .WithMany(k => k.Boden)
                .HasForeignKey(b => b.KavelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}