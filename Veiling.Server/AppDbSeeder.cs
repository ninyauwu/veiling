using System.Linq;
using Veiling.Server.Models;

namespace Veiling.Server
{
    public static class AppDbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // locaties
            if (!context.Locaties.Any())
            {
                context.Locaties.AddRange(
                    new Locatie { Naam = "Amsterdam", KlokId = 1, Actief = false },
                    new Locatie { Naam = "Rotterdam", KlokId = 2, Actief = true },
                    new Locatie { Naam = "Delft", KlokId = 3, Actief = true }
                );
            }
            else
            {
                // Update bestaande data
                var amsterdam = context.Locaties.FirstOrDefault(l => l.Naam == "Amsterdam");
                if (amsterdam != null) amsterdam.Actief = true;

                var rotterdam = context.Locaties.FirstOrDefault(l => l.Naam == "Rotterdam");
                if (rotterdam != null) rotterdam.Actief = false;

                var delft = context.Locaties.FirstOrDefault(l => l.Naam == "Delft");
                if (delft != null) delft.Actief = true;
            }

            // bedrijven, leveranciers, kavels
            if (!context.Kavels.Any())
            {
                var royalFlora = new Bedrijf
                {
                    Bedrijfscode = 0,
                    Bedrijfsnaam = "RoyalFlora BV",
                    KVKnummer = 13414514,
                };

                context.Bedrijven.Add(royalFlora);

                var leverancier = new Leverancier
                {
                    Bedrijf = royalFlora,
                    IndexOfReliabilityOfInformation = "A",
                };

                context.Leveranciers.Add(leverancier);

                context.Kavels.AddRange(
                    new Kavel
                    {
                        Naam = "Tulpen",
                        Beschrijving = "In 1962 werd ik aangevallen door beren.<br/>Nu vertrouw ik dus geen beren meer.",
                        MinimumPrijs = 0.22F,
                        MaximumPrijs = 0.97F,
                        Kavelkleur = "87335F",
                        Leverancier = leverancier,
                        StageOfMaturity = "24",
                        Keurcode = "A2",
                        Fustcode = 13914,
                    },
                    new Kavel
                    {
                        Naam = "Rozen",
                        Beschrijving = "Hey Vsauce, Michael here.",
                        MinimumPrijs = 0.12F,
                        MaximumPrijs = 0.40F,
                        Kavelkleur = "AAFF33",
                        Leverancier = leverancier,
                        StageOfMaturity = "20",
                        Keurcode = "A1",
                        Fustcode = 12354,
                    }
                );
            }
            
            context.SaveChanges();
        }
    }
}
