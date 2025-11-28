using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Veiling.Server.Models;

namespace Veiling.Server
{
    public class AppDbSeeder
    {
        public static async Task Seed(
            AppDbContext context,
            UserManager<Gebruiker> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var now = DateTime.UtcNow;

            // Locaties
            var amsterdam = new Locatie
            {
                Naam = "Amsterdam",
                KlokId = 1,
                Actief = true
            };

            var rotterdam = new Locatie
            {
                Naam = "Rotterdam",
                KlokId = 2,
                Actief = false
            };

            var delft = new Locatie
            {
                Naam = "Delft",
                KlokId = 3,
                Actief = true
            };

            context.Locaties.AddRange(amsterdam, rotterdam, delft);
            context.SaveChanges();

            // Bedrijven
            var bedrijf1 = new Bedrijf
            {
                Bedrijfsnaam = "Bloemen BV",
                KVKnummer = 12345678
            };

            var bedrijf2 = new Bedrijf
            {
                Bedrijfsnaam = "Flora Wholesale",
                KVKnummer = 87654321
            };

            context.Bedrijven.AddRange(bedrijf1, bedrijf2);
            context.SaveChanges();

            //Create rolles 
            foreach (var roleName in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Gebruikers
            var gebruiker1 = new Gebruiker
            {
                UserName = "jan@bloemen.nl",
                Email = "jan@bloemen.nl",
                Name = "Jan Jansen",
                PhoneNumber = "612345678",
                Bedrijfsbeheerder = true,
                Geverifieerd = true,
                BedrijfId = bedrijf1.Bedrijfscode
            };

            var gebruiker2 = new Gebruiker
            {
                UserName = "marie@flora.nl",
                Email = "marie@flora.nl",
                Name = "Marie Pieters",
                PhoneNumber = "687654321",
                Bedrijfsbeheerder = false,
                Geverifieerd = true,
                BedrijfId = bedrijf2.Bedrijfscode
            };


            //Give default password
            if (await userManager.FindByEmailAsync(gebruiker1.Email) == null)
                await userManager.CreateAsync(gebruiker1, "Password123!");

            if (await userManager.FindByEmailAsync(gebruiker2.Email) == null)
                await userManager.CreateAsync(gebruiker2, "Password123!");

            //Reload users
            gebruiker1 = await userManager.FindByEmailAsync(gebruiker1.Email);
            gebruiker2 = await userManager.FindByEmailAsync(gebruiker2.Email);


            //Give roles
            await userManager.AddToRoleAsync(gebruiker1, nameof(Role.Veilingmeester));
            await userManager.AddToRoleAsync(gebruiker2, nameof(Role.Administrator));


            // Veilingmeesters
            var veilingmeester1 = new Veilingmeester
            {
                GebruikerId = gebruiker1.Id,
                AantalVeilingenBeheerd = 5
            };

            context.Veilingmeesters.Add(veilingmeester1);
            context.SaveChanges();


            // Leveranciers
            var leverancier1 = new Leverancier
            {
                BedrijfId = bedrijf1.Bedrijfscode,
                Bedrijf = bedrijf1,
                IndexOfReliabilityOfInformation = "A"
            };

            context.Leveranciers.Add(leverancier1);
            context.SaveChanges();


            // veilingen
            var amsterdamVeiling = new Models.Veiling
            {
                Naam = "Amsterdam Ochtend Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-1),
                EndTijd = now.AddHours(2),
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = amsterdam.Id
            };

            var rotterdamVeiling1 = new Models.Veiling
            {
                Naam = "Rotterdam Middag Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(5),
                EndTijd = now.AddHours(7),
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = rotterdam.Id
            };

            var delftVeiling = new Models.Veiling
            {
                Naam = "Delft Avond Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-2),
                EndTijd = now.AddMinutes(45),
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = delft.Id
            };

            context.Veilingen.AddRange(amsterdamVeiling, rotterdamVeiling1, delftVeiling);
            context.SaveChanges();


            // Kavels
            var kavel1 = new Kavel
            {
                Naam = "Rode Rozen",
                Beschrijving = "Premium rode rozen",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 15.0f,
                MaximumPrijs = 25.0f,
                Minimumhoeveelheid = 10,
                Foto = "/images/rozen.jpg",
                Kavelkleur = "FF0000",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 20,
                LengteVanBloemen = 60.0f,
                GewichtVanBloemen = 500.0f,
                StageOfMaturity = "Bloeiend",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 123,
                GeldPerTickCode = "0.5",
                VeilingId = amsterdamVeiling.Id,
                LeverancierId = leverancier1.Id
            };

            context.Kavels.Add(kavel1);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
            Console.WriteLine($"Current time: {now}");
            Console.WriteLine($"Amsterdam veiling eindigt over: {(amsterdamVeiling.EndTijd - now).TotalHours:F1} uur");
            Console.WriteLine($"Rotterdam veiling begint over: {(rotterdamVeiling1.StartTijd - now).TotalHours:F1} uur");
            Console.WriteLine($"Delft veiling eindigt over: {(delftVeiling.EndTijd - now).TotalMinutes:F0} minuten");
        }
    }
}