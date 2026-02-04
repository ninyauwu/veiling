using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Veiling.Server;
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
            //clear db
            context.Boden.RemoveRange(context.Boden);
            context.Kavels.RemoveRange(context.Kavels);
            context.Veilingen.RemoveRange(context.Veilingen);
            context.Veilingmeesters.RemoveRange(context.Veilingmeesters);
            context.Leveranciers.RemoveRange(context.Leveranciers);
            context.Gebruikers.RemoveRange(context.Gebruikers);
            context.Bedrijven.RemoveRange(context.Bedrijven);
            context.Locaties.RemoveRange(context.Locaties);
            context.SaveChanges();

            var now = DateTime.UtcNow;

            // Locaties
            var aalsmeer = new Locatie
            {
                Naam = "Aalsmeer",
                KlokId = 1,
                Actief = true
            };

            var naaldwijk = new Locatie
            {
                Naam = "Naaldwijk",
                KlokId = 2,
                Actief = false
            };

            var rijnsburg = new Locatie
            {
                Naam = "Rijnsburg",
                KlokId = 3,
                Actief = true
            };

            var eelde = new Locatie
            {
                Naam = "Eelde",
                KlokId = 4,
                Actief = false
            };

            var rhein_maas = new Locatie
            {
                Naam = "Rhein-Maas",
                KlokId = 5,
                Actief = false
            };

            context.Locaties.AddRange(aalsmeer, naaldwijk, rijnsburg, eelde, rhein_maas);
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

            var gebruiker3 = new Gebruiker
            {
                UserName = "verkoper@verkopen.nl",
                Email = "verkoper@verkopen.nl",
                Name = "Verkoper Verkopen",
                PhoneNumber = "612345678",
                Bedrijfsbeheerder = true,
                Geverifieerd = true,
                BedrijfId = bedrijf1.Bedrijfscode
            };
            
            var gebruiker4 = new Gebruiker
            {
                UserName = "veiling@meester.nl",
                Email = "veiling@meester.nl",
                Name = "Veiling Meester",
                PhoneNumber = "612345678",
                Bedrijfsbeheerder = true,
                Geverifieerd = true,
                BedrijfId = bedrijf1.Bedrijfscode
            };

            var gebruiker5 = new Gebruiker
            {
                UserName = "bedrijfs@vertegenwoordiger.nl",
                Email = "bedrijfs@vertegenwoordiger.nl",
                Name = "bedrijfs vertegenwoordiger",
                PhoneNumber = "612345678",
                Bedrijfsbeheerder = true,
                Geverifieerd = true,
                BedrijfId = bedrijf1.Bedrijfscode
            };

            //Give default password
            if (await userManager.FindByEmailAsync(gebruiker1.Email) == null)
                await userManager.CreateAsync(gebruiker1, "Password123!");

            if (await userManager.FindByEmailAsync(gebruiker2.Email) == null)
                await userManager.CreateAsync(gebruiker2, "Password123!");

            if (await userManager.FindByEmailAsync(gebruiker3.Email) == null)
                await userManager.CreateAsync(gebruiker3, "Password123!");
            
            if (await userManager.FindByEmailAsync(gebruiker4.Email) == null)
                await userManager.CreateAsync(gebruiker4, "Password123!");
            
            if (await userManager.FindByEmailAsync(gebruiker5.Email) == null)
                await userManager.CreateAsync(gebruiker5, "Password123!");

            //Reload users
            gebruiker1 = await userManager.FindByEmailAsync(gebruiker1.Email);
            gebruiker2 = await userManager.FindByEmailAsync(gebruiker2.Email);
            gebruiker3 = await userManager.FindByEmailAsync(gebruiker3.Email);
            gebruiker4 = await userManager.FindByEmailAsync(gebruiker4.Email);
            gebruiker5 = await userManager.FindByEmailAsync(gebruiker5.Email);


            //Give roles
            await userManager.AddToRoleAsync(gebruiker1, nameof(Role.Gebruiker));
            await userManager.AddToRoleAsync(gebruiker2, nameof(Role.Administrator));
            await userManager.AddToRoleAsync(gebruiker3, nameof(Role.Leverancier));
            await userManager.AddToRoleAsync(gebruiker4, nameof(Role.Veilingmeester));
            await userManager.AddToRoleAsync(gebruiker5, nameof(Role.Bedrijfsvertegenwoordiger));
            


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

            var leverancier2 = new Leverancier
            {
                BedrijfId = bedrijf2.Bedrijfscode,
                Bedrijf = bedrijf2,
                IndexOfReliabilityOfInformation = "B"
            };

            context.Leveranciers.Add(leverancier1);
            context.Leveranciers.Add(leverancier2);
            context.SaveChanges();


            
            var today = DateTime.Today;
            
            // veilingen
            var amsterdamVeiling = new Models.Veiling
            {
                Naam = "Aalsmeer Ochtend Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(-1), // Started 1 uur geleden
                EndTijd = now.AddHours(2), // eindigd morgen 11 uur sochtends
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = aalsmeer.Id
            };

            var rotterdamVeiling1 = new Models.Veiling
            {
                Naam = "Naaldwijk Middag Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddMinutes(-30), // Started 30 min geleden
                EndTijd = now.AddHours(1.5), 
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = naaldwijk.Id
            };

            var delftVeiling = new Models.Veiling
            {
                Naam = "Rijnsburg Avond Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(3),  // Start over 3 uur
                EndTijd = now.AddHours(5),
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = rijnsburg.Id
            };
            
            var eeldeVeiling = new Models.Veiling
            {
                Naam = "Eelde Bloemen Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(6),   // Start over 6 uur
                EndTijd = now.AddHours(8),     // Eindigt over 8 uur
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = eelde.Id
            };
            
            var rheinMaasVeiling = new Models.Veiling
            {
                Naam = "Rhein-Maas Avond Veiling",
                Klokduur = 5.0f,
                StartTijd = now.AddHours(10),  // Start over 10 uur
                EndTijd = now.AddHours(12),    // Eindigt over 12 uur
                GeldPerTickCode = 0.5f,
                VeilingmeesterId = veilingmeester1.Id,
                LocatieId = rhein_maas.Id
            };

            context.Veilingen.AddRange(amsterdamVeiling, rotterdamVeiling1, delftVeiling,     eeldeVeiling, 
                rheinMaasVeiling);
            context.SaveChanges();


            // Kavels
            var kavel1 = new Kavel
            {
                Naam = "Nederlandse Rode Rozen",
                Beschrijving = "Premium rode rozen",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 15.0f,
                MaximumPrijs = 25.0f,
                Minimumhoeveelheid = 10,
                Foto = "https://thypix.com/wp-content/uploads/bouquets-of-roses-64.jpg",
                Kavelkleur = "#FF0000",
                Karnummer = 1,
                Rijnummer = 1,
                LeverancierId = leverancier1.Id,
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
            };

            var kavel2 = new Kavel
            {
                Naam = "Paarse Franse Tulpen",
                Beschrijving = "Matige Franse tulpen met de neiging om tot stof uit elkaar te vallen, extra goedkoop",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 0.1f,
                MaximumPrijs = 0.8f,
                Minimumhoeveelheid = 20,
                Foto = "https://rurallivingtoday.com/wp-content/uploads/Purple-Tulips.jpg",
                Kavelkleur = "#a200ff",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 24,
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
            
            var kavel3 = new Kavel
            {
                Naam = "Paarse Franse Tulpen",
                Beschrijving = "Matige Franse tulpen met de neiging om tot stof uit elkaar te vallen, extra goedkoop",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 0.1f,
                GekochtPrijs = 0.5f,
                MaximumPrijs = 0.8f,
                Minimumhoeveelheid = 20,
                Foto = "https://cdn.myonlinestore.eu/940bbdce-6be1-11e9-a722-44a8421b9960/image/cache/full/5fbed4dc3e621398e9829862e6d1670f1ab6f0aa.jpg?20260127102802",
                Kavelkleur = "800080",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 24,
                LengteVanBloemen = 60.0f,
                GewichtVanBloemen = 500.0f,
                StageOfMaturity = "Bloeiend",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 123,
                GeldPerTickCode = "0.5",
                VeilingId = rotterdamVeiling1.Id,
                LeverancierId = leverancier1.Id
            };

            var kavel4 = new Kavel
            {
                Naam = "Zonnige Zonebloemen",
                Beschrijving = "Deze bloemen worden heel lang gebraad in de zon voordat ze geteeld worden",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 20.0f,
                GekochtPrijs = 30.0f,
                MaximumPrijs = 80.0f,
                Minimumhoeveelheid = 20,
                Foto = "https://www.homeandgarden.nl/app/uploads/2014/08/zonnebloem-scaled.jpg",
                Kavelkleur = "FFF017",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 24,
                LengteVanBloemen = 60.0f,
                GewichtVanBloemen = 500.0f,
                StageOfMaturity = "Bloeiend",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 123,
                GeldPerTickCode = "0.5",
                VeilingId = rotterdamVeiling1.Id,
                LeverancierId = leverancier1.Id
            };

            var kavel5 = new Kavel
            {
                Naam = "Zonnige Zonebloemen",
                Beschrijving = "Deze bloemen worden heel lang gebraad in de zon voordat ze geteeld worden",
                ArtikelKenmerken = "Lang, sterk",
                MinimumPrijs = 20.0f,
                GekochtPrijs = 70.0f,
                MaximumPrijs = 80.0f,
                Minimumhoeveelheid = 20,
                Foto = "https://www.jansenzaden.nl/cdn/shop/collections/zonnebloemen.jpg?v=1741349258&width=1280",
                Kavelkleur = "FFF017",
                Karnummer = 1,
                Rijnummer = 1,
                HoeveelheidContainers = 50,
                AantalProductenPerContainer = 24,
                LengteVanBloemen = 60.0f,
                GewichtVanBloemen = 500.0f,
                StageOfMaturity = "Bloeiend",
                NgsCode = 'A',
                Keurcode = "A1",
                Fustcode = 123,
                GeldPerTickCode = "0.5",
                VeilingId = rotterdamVeiling1.Id,
                LeverancierId = leverancier2.Id
            };

            context.Kavels.Add(kavel1);
            context.Kavels.Add(kavel2);
            context.Kavels.Add(kavel3);
            context.Kavels.Add(kavel4);
            context.Kavels.Add(kavel5);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
            Console.WriteLine($"Current time: {now}");
            Console.WriteLine($"Amsterdam veiling eindigt over: {(amsterdamVeiling.EndTijd - now).TotalHours:F1} uur");
            Console.WriteLine($"Rotterdam veiling begint over: {(rotterdamVeiling1.StartTijd - now).TotalHours:F1} uur");
            Console.WriteLine($"Delft veiling eindigt over: {(delftVeiling.EndTijd - now).TotalMinutes:F0} minuten");
        }
    }
}
